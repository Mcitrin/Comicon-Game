using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public InputMan inputMan;
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

    bool dive;
    float diveTime;
    float diveCoolDown = .25f;
    
    // Use this for initialization
    void OnEnable()
    {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
        distToGround = GetComponent<BoxCollider2D>().bounds.extents.y + 0.03f;
    }

    public bool IsGrounded()
    {
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x,transform.position.y) -Vector2.up * distToGround, Color.red);
        return Physics2D.Raycast(transform.position, -Vector2.up, distToGround, 1 << LayerMask.NameToLayer("Ground"));
       
    }

    void Input()
    {
        Move();
        Aiming();
        CalcPower();
        CalcJump();
        if (dive)
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
            currentVelocity.x = inputMan.Move(PlayerNumber) * 15.0f;
            
        }
        else
        {
            currentVelocity.x = inputMan.Move(PlayerNumber) * 40.0f;
        }

        if (!dive)
        {   
            characterController.Move(currentVelocity.x,PlayerNumber);
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

    void HandleDive()
    {
        if (diveTime == 0)
        {
            diveTime = Time.time + diveCoolDown;
        }
        else if (diveTime <= Time.time)
        {
            dive = false;
            diveTime = 0;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
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
        else
        {
            animationController.speed = 0;
        }
    }
}
