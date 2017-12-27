using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    // refrence to the inputmanager script attached to the singelton gameManager object
    public InputMan inputMan;
    // the players nubmer 1 or 2
    public int PlayerNumber;

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
    public CharacterController characterController;

    // handel to the animation controller delegate
    public AnimationController animationController;

    // handel to the apperance delegate (changes sprite colors for cusimization)
    public Appearance appearance;

    float jumpHoldTime;
    float jumpHoldLimit = .25f;

    // are you diveing
    bool dive;
    // the time your dive will stop at (time.time + diveLength)
    float diveEndTime;
    // how long your dive will last
    float diveLength = 1;

  public enum PlayerState
    {
        GROUNDED,
        DIVEING,
        JUMP1,
        JUMP2,
        FLOATING,
        FLOATING2
    }

    public PlayerState playerState = PlayerState.GROUNDED;

    // Use this for initialization
    void Start()
    {
        inputMan = GameManager.gameManager.inputMan;
        characterController.playerNum = PlayerNumber;
    }


   

    // calls all the function related to player input
    void Input()
    {
        // handels aiming
        Aiming();

        // determins how hard your going to hit the ball ie if your charging
        CalcPower();

        if(playerState == PlayerState.JUMP1)
        {
            if (!runningCalcJump)
            {
                runningCalcJump = true;
                StartCoroutine(CalcJumpHeight());
            }
        }

        // if your diving run this
        if (!dive)
        {
            Move();
        }

    }

    void Animate()
    {
        // send information to the animation manager so it knows wich animation to play
        animationController.speed = Mathf.Abs(inputMan.Move(PlayerNumber));
        animationController.isGrounded = characterController.grounded;
        animationController.chargeing = chargeing;
    }

    void Move()
    {
       if(playerState == PlayerState.JUMP2)
       {
           characterController.Jump2();
           playerState = PlayerState.FLOATING2;
       }

        // if were in the air or chargin move slower
        if (playerState != PlayerState.GROUNDED || chargeing)
        {
            characterController.Move((inputMan.Move(PlayerNumber)), true);
        }
        // else move normal
        else
        {
            characterController.Move((inputMan.Move(PlayerNumber)), false);
        }

        //if (inputMan.Down(PlayerNumber) && inputMan.Charge(PlayerNumber) && !dive)
        //{
        //    dive = true;
        //    StartCoroutine(HandleDive());
        //}

    }

    // determin were were aming with this right stick our mouse
    void Aiming()
    {
        // if were aimng ie the right stick is not at reset or zero
        if (inputMan.Aim(PlayerNumber) != Vector2.zero)
        {
            // get the angel from the input manager
            
            angleTo = inputMan.Aim(PlayerNumber);
            
            // this all keep the player from aging behind them
            // lock to front 180 arc
            if (PlayerNumber == 1)
            {
                if (angleTo.x < 0 && angleTo.y >= 0)
                    angleTo = new Vector3(0, 1);
                if (angleTo.x < 0 && angleTo.y <= 0)
                    angleTo = new Vector3(0, -1);
            }
            if (PlayerNumber == 2)
            {
                if (angleTo.x > 0 && angleTo.y >= 0)
                    angleTo = new Vector3(0, 1);
                if (angleTo.x > 0 && angleTo.y <= 0)
                    angleTo = new Vector3(0, -1);
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
    IEnumerator CalcJumpHeight()
    {
        jumpHoldTime += Time.deltaTime;

        if(jumpHoldTime >= jumpHoldLimit)
        {
            characterController.Jump("high");
            jumpHoldTime = 0;
            yield return new WaitForSeconds(.1f);
            playerState = PlayerState.FLOATING;
        }
        else if(inputMan.JumpRelease(PlayerNumber))
        {
            characterController.Jump("low");
            jumpHoldTime = 0;
            yield return new WaitForSeconds(.1f);
            playerState = PlayerState.FLOATING;
        }

        runningCalcJump = false;
        yield return new WaitForSeconds(0);
    }

    IEnumerator HandleDive()
    {
        playerState = PlayerState.FLOATING2;
        StartCoroutine(characterController.Dive(diveLength));
        yield return new WaitForSeconds(diveLength);
        dive = false;
    }

    void ManageState()
    {
        if (characterController.grounded && playerState != PlayerState.JUMP1)
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

        // if the games not paused recive input and allow animation
        if (!GameManager.paused)
        {
            Input();
            Animate();
            ManageState();
        }
        else
        {
            // tells the animation manger to stop walking
            animationController.speed = 0;
        }

    }
}
