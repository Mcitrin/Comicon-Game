﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    InputMan inputMan;
    public int PlayerNumber;
    float distToGround;
    float startTime;
    float chargeTime;
    bool chargeing = false;
    Vector3 currentVelocity;
    Vector3 angle;
    int power;
    public CharacterController characterController;
    public AnimationController animationController;
    public Appearance appearance;

    bool canJump;
    float maxJumpHeight = 7f;

    public int dash = 0;
    float dashTime;
    float dashCoolDown = .25f;


    Vector2 DoubleTapCount;
    float DoubleTapCoolDown = 0.5f;
    bool DoubleTapReset = false;
    
    // Use this for initialization
    void Start()
    {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + .03f, 1 << LayerMask.NameToLayer("Ground"));
    }

    void Input()
    {
        Move();
        Aiming();
        CalcPower();
        CalcJump();
        if (dash == 0)
        {
            CheckForDoubleTap();
        }
        else
        {
            HandleDash();
        }
        
    }

    void Animate()
    {
        animationController.speed = Mathf.Abs(inputMan.Move(PlayerNumber));
        animationController.isGrounded = IsGrounded();
        animationController.chargeing = chargeing;
    }

    void Move()
    {
        currentVelocity = this.GetComponent<Rigidbody>().velocity;

        if (canJump && inputMan.Jump(PlayerNumber))
        {
            characterController.jump(true,IsGrounded());
        }
        else
        {
            characterController.jump(false,IsGrounded());
        }

        if (!IsGrounded() || chargeing)
        {
            currentVelocity.x = inputMan.Move(PlayerNumber) * 5.0f;

        }
        else
        {
            currentVelocity.x = inputMan.Move(PlayerNumber) * 9.0f;
        }

        if (dash == 0)
        {
            characterController.Move(currentVelocity.x);
        }
        else // figure this headache out later
        {
            characterController.Move(dash * 15f);
        }

    }

    void Aiming()
    {
        if (inputMan.Aim(PlayerNumber) != Vector2.zero)
        {
            angle = inputMan.Aim(PlayerNumber);

            // lock to front 180
            if (PlayerNumber == 1)
            {
                if (angle.x < 0 && angle.y >= 0)
                    angle = new Vector3(0, 1);
                if (angle.x < 0 && angle.y <= 0)
                    angle = new Vector3(0, -1);
            }
            if (PlayerNumber == 2)
            {
                if (angle.x > 0 && angle.y >= 0)
                    angle = new Vector3(0, 1);
                if (angle.x > 0 && angle.y <= 0)
                    angle = new Vector3(0, -1);
            }
        }

        characterController.Aim(angle);
    }

    void CalcPower()
    {
        if (inputMan.Charge(PlayerNumber))
        {

            if (startTime == 0)
            {
                startTime = Time.time;
            }
            chargeTime = (Time.time - startTime);
        }

        if (chargeTime >= .2f)
        {

            chargeing = true;
        }
        else { chargeing = false; }

        if (!inputMan.Charge(PlayerNumber))
        {
            if (startTime != 0)// if the button was pressed befor were reading it is released
            {

                if (!IsGrounded())
                {
                    animationController.setTrigger("Smack");
                }
                else
                {
                    animationController.setTrigger("Set");
                }

                if (chargeTime >= .65)
                {
                    power = 2;
                }
                else
                {
                    power = 1;
                }
                StartCoroutine(characterController.setPower(power));
                StartCoroutine(appearance.Flash(power));
                chargeTime = 0;
                startTime = 0;
            }
        }
    }

    void CalcJump()
    {
        if (IsGrounded())
        {
            canJump = true;
        }

        if (transform.position.y >= maxJumpHeight)
        {
            canJump = false;
        }
    }

   void CheckForDoubleTap()
    {
        int horrizontalInput = Mathf.Clamp((int)(inputMan.Move(PlayerNumber) * 10), -1, 1);
        
        if (DoubleTapCoolDown > 0)
        {

            DoubleTapCoolDown -= 1 * Time.deltaTime;

        }
        else
        {
            DoubleTapCount = Vector2.zero;
            DoubleTapReset = false;
        }

        if (horrizontalInput == 1 ) // tap Right
        {
            if (DoubleTapCoolDown > 0 && DoubleTapCount.x == 1 && DoubleTapReset)
            {
                //Has double tapped
                DoubleTapCoolDown = 0;
                dash = 1;
            }
            else
            {
                // first tap
                DoubleTapCount.x = 1;
                DoubleTapCount.y = 0;
                DoubleTapCoolDown = 0.5f;
            }

            DoubleTapReset = false;
        }

        if (horrizontalInput == -1) // tap Left
        {
            if (DoubleTapCoolDown > 0 && DoubleTapCount.y == 1 && DoubleTapReset)
            {
                //Has double tapped
                DoubleTapCoolDown = 0;
                dash = -1;
            }
            else
            {
                // first tap
                DoubleTapCount.y = 1;
                DoubleTapCount.x = 0;
                DoubleTapCoolDown = 0.5f;
            }

            DoubleTapReset = false;
        }
        else if (horrizontalInput == 0)
        {
            if(DoubleTapCount.y > 0 || DoubleTapCount.x > 0)
            {
                DoubleTapReset = true;
            }
        }
    }

    void HandleDash()
    {
        if (dashTime == 0)
        {
            dashTime = Time.time + dashCoolDown;
        }
        else if (dashTime <= Time.time)
        {
            dash = 0;
            dashTime = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // pause check
        if (inputMan.Pause(PlayerNumber) && !GameManager.paused)
        {
            GameManager.paused = true;
        }
        else if (inputMan.Pause(PlayerNumber) && GameManager.paused)
        {
            GameManager.paused = false;
        }

        if (!GameManager.paused)
        {
            Input();
            Animate();
        }
    }
}
