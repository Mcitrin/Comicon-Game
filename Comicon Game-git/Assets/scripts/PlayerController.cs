﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    // refrence to the inputmanager script attached to the singelton gameManager object
    public InputMan inputMan;
    // the players nubmer 1 or 2
    public int PlayerNumber;
    // used for stilling if were touching the ground
    float distToGround;

    // the time that you started charging your shot at
    float startTime;

    // how long youve been charging for (time.time - startTime)
    float chargeTime;

    // werether or not your currently charging a shot
    bool chargeing = false;

    // the x velocity were going to pass to the charactrt controler to move te gameobject
    float currentVelocityX;

    // the angle your aming at to be passed to the character controller 
    Vector3 angle;

    // the power your hitting the ball with passed to the character controller 
    int power;

    // handel to the character controller delegate
    public CharacterController characterController;

    // handel to the animation controller delegate
    public AnimationController animationController;

    // handel to the apperance delegate (changes sprite colors for cusimization)
    public Appearance appearance;

    // can you current jump, (basically have you jumped hit and are you on the ground)
    bool canJump;

    bool grounded;

    int numJumps = 0;

    float maxJumpHeight = 7f;

    // theses arnt implemented yet but they handel the player diveing for the ball

    // are you diveing
    bool dive;
    // the time your dive will stop at (time.time + diveLength)
    float diveEndTime;
    // how long your dive will last
    float diveLength = .25f;

    enum PlayerState
    {
        GROUNDED,
        JUMP1,
        JUMP2,
        FLOATING,
        FLOATING2
    }

     PlayerState playerState = PlayerState.GROUNDED;

    // Use this for initialization
    void Start()
    {
        inputMan = GameManager.gameManager.inputMan;
        distToGround = GetComponent<BoxCollider2D>().bounds.extents.y + 0.03f;
    }


    // are we touching the ground
    public bool IsGrounded()
    {
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) - Vector2.up * distToGround, Color.red);
        return Physics2D.Raycast(transform.position, -Vector2.up, distToGround, 1 << LayerMask.NameToLayer("Ground"));
    }

    // calls all the function related to player input
    void Input()
    {
        // handels moving
        Move();
        // handels aiming
        Aiming();
        // determins how hard your going to hit the ball ie if your charging
        CalcPower();


        grounded = IsGrounded();
        // changes payer state
        ManageState();
        // if your diving run this
        if (dive)
        {
            HandleDive();
        }

    }

    void Animate()
    {
        // send information to the animation manager so it knows wich animation to play

        animationController.speed = Mathf.Abs(inputMan.Move(PlayerNumber));
        animationController.isGrounded = grounded;
        animationController.chargeing = chargeing;
    }

    void Move()
    {
        characterController.ApplyGravity(grounded);
        ManageState();

        if(playerState == PlayerState.JUMP1)
        {
            characterController.Jump();
        }

        if(playerState == PlayerState.JUMP2)
        {
            characterController.Jump();
            playerState = PlayerState.FLOATING2;
        }


        // if were in the air or chargin move slower
        if (!grounded || chargeing)
        {
            // set velocity to the input recived from input manager
            currentVelocityX = inputMan.Move(PlayerNumber) * 15.0f;

        }
        // else move normal
        else
        {
            currentVelocityX = inputMan.Move(PlayerNumber) * 40.0f;
        }

        // if were not diving pass out velocity to the character controller
        if (!dive)
        {
            characterController.Move(currentVelocityX, PlayerNumber);
        }

    }

    // determin were were aming with this right stick our mouse
    void Aiming()
    {
        // if were aimng ie the right stick is not at reset or zero
        if (inputMan.Aim(PlayerNumber) != Vector2.zero)
        {
            // get the angel from the input manager
            angle = inputMan.Aim(PlayerNumber);


            // this all keep the player from aging behind them
            // lock to front 180 arc
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

        // pass our angel to the character controller
        characterController.Aim(angle);
    }

    void CalcPower()
    {
        // the player currently pressing the button to hit the ball
        if (inputMan.Charge(PlayerNumber))
        {

            // if we werent already holding down the hit button record the time that we started holding the button at
            if (startTime == 0)
            {
                startTime = Time.time;
            }
            chargeTime = (Time.time - startTime);
        }

        // is wever been holding the hit button long enough were now charging
        if (chargeTime >= .2f)
        {

            chargeing = true;
        }
        else { chargeing = false; }

        // if the hit button is not currently beign pressd 
        if (!inputMan.Charge(PlayerNumber))
        {
            // if start time is not zero this means we have pressed the button down befor it has regesterd that it is not currently pressed
            // ie we have just done a smack
            if (startTime != 0)
            {

                // if were in the air play the animation for smacking the ball
                if (!IsGrounded())
                {
                    animationController.setTrigger("Smack");
                }
                // if were on the ground play the animation for setting the ball
                else
                {
                    animationController.setTrigger("Set");
                }

                // if weve charged long enough were doing a power hit
                if (chargeTime >= .65)
                {
                    power = 2;
                }
                // else its just a nomral hit
                else
                {
                    power = 1;
                }
                // tell the character controller we just smacked and give it the power of the smack
                StartCoroutine(characterController.setPower(power));
                // flash red if our power is 2 
                if (power == 2)
                    StartCoroutine(appearance.Flash());

                chargeTime = 0;
                startTime = 0;
            }
        }
    }

    void HandleDive()
    {
        // if were not currently diving
        if (diveEndTime == 0)
        {
            diveEndTime = Time.time + diveLength;
        }
        // if our dives over
        else if (diveEndTime <= Time.time)
        {
            dive = false;
            diveEndTime = 0;
        }
    }

    void ManageState()
    {
        if (grounded && playerState != PlayerState.JUMP1)
        {
            playerState = PlayerState.GROUNDED;
        }
        if (inputMan.JumpPress(PlayerNumber))
        {
                if(playerState == PlayerState.GROUNDED)
                playerState = PlayerState.JUMP1;
                if(playerState == PlayerState.FLOATING)
                playerState = PlayerState.JUMP2;
        }
        if(playerState == PlayerState.JUMP1 && (inputMan.JumpRelease(PlayerNumber) || transform.position.y >= maxJumpHeight))
        {
            playerState = PlayerState.FLOATING;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerState);

        // pause check
        if (inputMan.Pause(PlayerNumber) && !GameManager.paused)
        {
            GameManager.paused = true;
        }
        else if (inputMan.Pause(PlayerNumber) && GameManager.paused)
        {
            GameManager.paused = false;
        }


        // if the games not paused recive input and allow animation
        if (!GameManager.paused)
        {
            Input();
            Animate();
        }
        else
        {
            // tells the animation manger to stop walking
            animationController.speed = 0;
        }

    }
}