using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    // refrence to the inputmanager script attached to the singelton gameManager object
    public InputMan inputMan;
    // the players nubmer 1 or 2
    public int playerNumber;

    // the time that you started charging your shot at
    float startTime;

    // how long youve been charging for (time.time - startTime)
    float chargeTime;

    // werether or not your currently charging a shot
    bool chargeing = false;

    //if cacl jump is running
    bool runningCalcJump = false;

    // the angle your aming at to be passed to the character controller 
    Vector2 angle;
    // the angle to lerp to
    Vector2 angleTo = Vector3.zero;

    // the power your hitting the ball with passed to the character controller 
    int power;

    // handel to the character controller delegate
    CharacterController characterController;

    // handel to the animation controller delegate
    public AnimationController animationController;

    // handel to the apperance delegate (changes sprite colors for cusimization)
    public Appearance appearance;

    float jumpHoldTime;
    float jumpHoldLimit = 1.5f;
    float jumpHoldMin = .4f;

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

        // determins how hard your going to hit the ball
        CalcPower();

        Move();

        if (playerState == PlayerState.CHARGEJUMP)
        {
            if (!runningCalcJump)
            {
                runningCalcJump = true;
                CalcJumpHeight();
            }
        }

        sliders.SetValue("jump", jumpHoldTime / jumpHoldLimit);

        //Debug.Log(jumpHoldTime / jumpHoldLimit);
    }

    void Animate()
    {
        // send information to the animation manager so it knows wich animation to play
        animationController.speed = Mathf.Abs(inputMan.Move(playerNumber));
        animationController.isGrounded = characterController.grounded;
        animationController.chargeing = chargeing;
    }

    void Move()
    {
       //if(playerState == PlayerState.JUMP2)
       //{
       //    characterController.Jump2();
       //    playerState = PlayerState.FLOATING2;
       //}

        // if were in the air or chargin move slower
        if (playerState != PlayerState.GROUNDED || chargeing)
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


    void CalcPower()
    {
        // the player currently pressing the button to hit the ball
        if (inputMan.Charge(playerNumber))
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
        if (!inputMan.Charge(playerNumber))
        {
            // if start time is not zero this means we have pressed the button down befor it has regesterd that it is not currently pressed
            // ie we have just done a smack
            if (startTime != 0)
            {

                // if were in the air play the animation for smacking the ball
                if (playerState != PlayerState.GROUNDED)
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
    void CalcJumpHeight()
    {
        jumpHoldTime += Time.deltaTime;
        jumpHoldTime = Mathf.Clamp(jumpHoldTime, 0, jumpHoldLimit);

        //if(jumpHoldTime >= jumpHoldLimit)
        //{
        //    characterController.Jump("high");
        //    jumpHoldTime = 0;
        //    yield return new WaitForSeconds(.1f);
        //    playerState = PlayerState.FLOATING;
        //}
        //else if(inputMan.JumpRelease(playerNumber))
        //{
        //    characterController.Jump("low");
        //    jumpHoldTime = 0;
        //    yield return new WaitForSeconds(.1f);
        //    playerState = PlayerState.FLOATING;
        //}

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

        runningCalcJump = false;
        return;
        //yield return new WaitForSeconds(0);
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
