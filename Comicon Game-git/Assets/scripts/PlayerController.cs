using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

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

    // Use this for initialization
    void Awake () {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;
    }

    public bool IsGrounded()
    {
        //RaycastHit hitInfo;
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f, 1 << LayerMask.NameToLayer("Ground"));

    }

    void Input()
    {
        currentVelocity = this.GetComponent<Rigidbody>().velocity;
        if (inputMan.Jump(PlayerNumber) && IsGrounded())
        {
            //characterController.jump(new Vector3(0, 15, 0));
            currentVelocity.y = 15;
        }
        if (!IsGrounded() || chargeing)
        {
            currentVelocity.x = inputMan.Move(PlayerNumber) * 5.0f;//2.5f;
        }
        else
        {
            currentVelocity.x = inputMan.Move(PlayerNumber) * 9.0f;//5
        }

        characterController.Move(currentVelocity);

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
        CalcPower();
    }

    void Animate()
    {
        animationController.speed = Mathf.Abs(inputMan.Move(PlayerNumber));
        animationController.isGrounded = IsGrounded();
        animationController.chargeing = chargeing;
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


    // Update is called once per frame
    void Update () {
        // pause check
        if (inputMan.Pause(PlayerNumber) && !GameManager.paused)
        {
            GameManager.paused = true;
        }
        else if (inputMan.Pause(PlayerNumber) && GameManager.paused)
        {
            GameManager.paused = false;
        }

        if(!GameManager.paused)
        {
            Input();
            Animate();
        }
    }
}
