using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    // refrence to the inputmanager script attached to the singelton gameManager object
    public InputMan inputMan;
    // the players nubmer 1 or 2
    public int playerNumber;

    // werether or not your currently charging a shot
    bool chargeingHit = false;
    //if cacl jump is running
    bool chargeingJump = false;

    // the angle your aming at to be passed to the character controller 
    Vector2 angle;
    // the angle to lerp to
    Vector2 angleTo = Vector3.zero;

    // the power your hitting the ball with passed to the character controller 
    //int power;

    // handel to the character controller delegate
    CharacterController characterController;

    // handel to the animation controller delegate
    public AnimationController animationController;

    // handel to the apperance delegate (changes sprite colors for cusimization)
    public Appearance appearance;

    float jumpHoldTime;
    float jumpHoldLimit = 1.5f;
    float jumpHoldMin = .4f;

    float hitHoldTime;
    float hitHoldLimit = 1.5f;
    float hitHoldMin = .4f;

    // are you diveing
    bool dive;
    // the time your dive will stop at (time.time + diveLength)
    float diveEndTime;
    // how long your dive will last
    float diveLength = 1;

    public Sliders sliders;

  public enum PlayerState
    {
        GROUNDED,
        CHARGEJUMP,
        JUMP,
        //JUMP2,
        FLOATING,
        //FLOATING2
    }

    public PlayerState playerState = PlayerState.GROUNDED;

    // Use this for initialization
    void Start()
    {
        inputMan = GameManager.gameManager.inputMan;
        characterController = GetComponent<CharacterController>();
        characterController.playerNum = playerNumber;

        characterController.jumpApexReached += delegate (int PlayerNumber)
        {
            if(PlayerNumber == playerNumber && playerState == PlayerState.JUMP)
            {
                playerState = PlayerState.FLOATING;
            }
        };
    }


   

    // calls all the function related to player input
    void Input()
    {
        // handels aiming
        Aiming();
        Move();

        // determins how hard your going to hit the ball
        if(chargeingHit)
        {
            CalcHitPower();
        }
        
        if (playerState == PlayerState.CHARGEJUMP)
        {
            if (!chargeingJump)
            {
                chargeingJump = true;
                CalcJumpHeight();
            }
        }
        
        sliders.SetValue("jump", jumpHoldTime / jumpHoldLimit);
        sliders.SetValue("hit", hitHoldTime / hitHoldLimit);
    }

    void Animate()
    {
        // send information to the animation manager so it knows wich animation to play
        animationController.speed = Mathf.Abs(inputMan.Move(playerNumber));
        animationController.isGrounded = characterController.grounded;
        animationController.chargeingHit = chargeingHit;
    }

    void Move()
    {
       //if(playerState == PlayerState.JUMP2)
       //{
       //    characterController.Jump2();
       //    playerState = PlayerState.FLOATING2;
       //}

        if((playerState == PlayerState.JUMP || playerState == PlayerState.FLOATING) && inputMan.Down(playerNumber))
        {
            characterController.GMultiplier = 5;
        }
        else
        {
            characterController.GMultiplier = 1;
        }
        
        // if were in the air or chargin move slower
        if (playerState != PlayerState.GROUNDED || chargeingHit)
        {
            characterController.Move((inputMan.Move(playerNumber)), true);
        }
        // else move normal
        else
        {
            characterController.Move((inputMan.Move(playerNumber)), false);
        }

    }

    void Aiming()
    {
        // if were aimng ie the right stick is not at reset or zero
        if (inputMan.Aim(playerNumber) != Vector2.zero)
        {
            // get the angelTo from the input manager
            angleTo = inputMan.Aim(playerNumber);
            
            // lock to front arc
            if (playerNumber == 1)
            {
                if (angleTo.x < 0 && angleTo.y >= 0)
                    angleTo = new Vector3(0, 1);
                if (angleTo.x < .3f && angleTo.y <= 0)
                    angleTo = new Vector3(.3f, -1);
            }
            if (playerNumber == 2)
            {
                if (angleTo.x > 0 && angleTo.y >= 0)
                    angleTo = new Vector3(0, 1);
                if (angleTo.x > .3f && angleTo.y <= 0)
                    angleTo = new Vector3(.3f, -1);
            }

            float xPos = Linear(angle.x, angleTo.x, Time.deltaTime);
            float yPos = Linear(angle.y, angleTo.y, Time.deltaTime);
            angle = new Vector2(xPos, yPos).normalized;
        }

        // pass our angel to the character controller
        characterController.Aim(angle.normalized);
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value*10);
    }


    void CalcHitPower()
    {
        hitHoldTime += Time.deltaTime;
        hitHoldTime = Mathf.Clamp(hitHoldTime, 0, hitHoldLimit);

        if (hitHoldTime < hitHoldMin)
        {
            hitHoldTime = hitHoldMin;
        }
        if (inputMan.ChargeRelease(playerNumber))
        {
            
            if (hitHoldTime < hitHoldMin)
            {
                hitHoldTime = hitHoldMin;
            }
            if (playerState != PlayerState.GROUNDED)
            {
                animationController.setTrigger("Smack");
            }
            else
            {
                animationController.setTrigger("Set");
                Debug.Log("Set");
            }
            characterController.setPower(hitHoldTime / hitHoldLimit);
            //if (power == 2)
                //StartCoroutine(appearance.Flash());
            hitHoldTime = 0;
            chargeingHit = false;
        }
        return;  
    }
    void CalcJumpHeight()
    {
        jumpHoldTime += Time.deltaTime;
        jumpHoldTime = Mathf.Clamp(jumpHoldTime, 0, jumpHoldLimit);

        if(inputMan.JumpRelease(playerNumber))
        {
            if(jumpHoldTime < jumpHoldMin)
            {
                jumpHoldTime = jumpHoldMin;
            }
            playerState = PlayerState.JUMP;
            characterController.Jump(jumpHoldTime / jumpHoldLimit);
            jumpHoldTime = 0;
        }

        chargeingJump = false;
        return;
    }

    void ManageState()
    {
        if (characterController.grounded && playerState != PlayerState.JUMP 
                                         && playerState != PlayerState.CHARGEJUMP)
        {
            playerState = PlayerState.GROUNDED;
        }
        if (inputMan.JumpPress(playerNumber))
        {
                if(playerState == PlayerState.GROUNDED)
                playerState = PlayerState.CHARGEJUMP;
                //if(playerState == PlayerState.FLOATING)
                //playerState = PlayerState.JUMP2;
        }
        if (!chargeingHit && inputMan.Charge(playerNumber))
        {
            chargeingHit = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // pause check
        if (inputMan.Pause(playerNumber) && !GameManager.paused)
        {
            GameManager.paused = true;
        }
        else if (inputMan.Pause(playerNumber) && GameManager.paused)
        {
            GameManager.paused = false;
        }

        // if the games not paused recive input and allow animation
        if (!GameManager.paused)
        {
            Input();
            Animate();
            ManageState();

            if (animationController.Animator.speed < 1)
                animationController.Animator.speed = 1;
        }
        else
        {
            if (animationController.Animator.speed > 0)
                animationController.Animator.speed = 0;
        }

    }
}
