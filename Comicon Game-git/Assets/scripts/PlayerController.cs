using UnityEngine;
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

    public int dive = 0;
    float diveTime;
    float diveCoolDown = .25f;


    Vector2 DoubleTapCount;
    float DoubleTapCoolDown = 0.5f;
    bool DoubleTapReset = false;

    bool running = false;
    
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
        if (dive == 0)
        {
            CheckForDoubleTap();
        }
        else
        {
            HandleDive();
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
            running = false;
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

        if (dive == 0)
        {   if (!running)
            {
                characterController.Move(currentVelocity.x);
            }
            else
            {
                characterController.Move(currentVelocity.x * 1.75f);
            }
        }
        else // figure this headache out later
        {
            characterController.Move(dive * 15f);
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
                running = true;
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
                running = true;
            }
            else
            {
                // first tap
                DoubleTapCount.y = 1;
                DoubleTapCount.x = 0;
                DoubleTapCoolDown = 0.25f;
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

    void HandleDive()
    {
        if (diveTime == 0)
        {
            diveTime = Time.time + diveCoolDown;
        }
        else if (diveTime <= Time.time)
        {
            dive = 0;
            diveTime = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (PlayerNumber == 1)
            Debug.Log(running);

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
        else
        {
            animationController.speed = 0;
        }
    }
}
