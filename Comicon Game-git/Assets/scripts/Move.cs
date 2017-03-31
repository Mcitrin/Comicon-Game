using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{

    InputMan inputMan;
    AnimationManager animationManager;
    float distToGround;

    public Animator animator;
    public int PlayerNumber;
    public GameObject arrow;
    public GameObject hand;
    public SpriteRenderer HairSprite;
    public SpriteRenderer ShortsSprite;
    public SpriteRenderer StripeSprite;
    public Vector3 angle;
    float startTime;
    public int power;
    public float chargeTime;
    public Ball ball;

    //bool setting = false;
    bool chargeing = false;
    Color[] colors = new Color[6];

    // Use this for initialization
    void Awake()
    {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;
    }

     void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            colors[i] = gameObject.GetComponentsInChildren<SpriteRenderer>()[i].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        Input();
        
      if(!IsGrounded())
      this.GetComponent<Rigidbody>().velocity += Vector3.down *.15f;

       // Debug.Log(angle.x);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);
    }

    void Input()
    {
        if (inputMan.Jump(PlayerNumber) && IsGrounded())
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 15, 0);//7.5f;
        }

        Vector3 curVel = this.GetComponent<Rigidbody>().velocity;
        if (!IsGrounded() || chargeing)
        {
            curVel.x = inputMan.Move(PlayerNumber) * 5.0f;//2.5f;
        }
        else
        {
            curVel.x = inputMan.Move(PlayerNumber) * 9.0f;//5
        }

        this.GetComponent<Rigidbody>().velocity = curVel;

        if (inputMan.Aim(PlayerNumber) != Vector2.zero)
        {
            
            
            angle = inputMan.Aim(PlayerNumber);

            // lock to front 180
            if(PlayerNumber == 1)
            {
                if (angle.x < 0 && angle.y >= 0)
                    angle = new Vector3(0,1);
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
        
        
        arrow.transform.position = angle * 2 + this.transform.position;

        CalcPower();
    }

    void Animate()
    {
        animator.SetBool("Jump", !IsGrounded());

        animator.SetFloat("Speed",Mathf.Abs(inputMan.Move(PlayerNumber)));

        //animator.SetBool("Still", !chargeing);

        if (IsGrounded())
        {
            animator.SetBool("ChargeSmack", false);
            animator.SetBool("ChargeSet", chargeing);
        }
        else
        {
            animator.SetBool("ChargeSmack", chargeing);
            animator.SetBool("ChargeSet", false);
        }
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
                    animator.SetTrigger("Smack");
                }
                else
                {
                    animator.SetTrigger("Set");
                }

                if (chargeTime >= .75)
                {
                    power = 2;
                }
                else
                {
                    power = 1;
                }
                    StartCoroutine(Flash(power));

             //if (Vector3.Distance(new Vector2(this.transform.position.x, this.transform.position.y)
             //    , ball.transform.position) <= distance || ball.HeldBy == gameObject)
             // {
             //     ball.HitBall((int)power, angle, PlayerNumber, !IsGrounded());
             // }


             

               //   if(ball.CollidingPlayer == gameObject)
               // {
               //     ball.HitBall((int)power, angle, PlayerNumber, !IsGrounded());
               // }

                chargeTime = 0;
                startTime = 0;
                
            }
        }
    }
    IEnumerator Flash(int pow)
    {
        if (pow == 2)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    gameObject.GetComponentsInChildren<SpriteRenderer>()[j].color = Color.red;
                }
                yield return new WaitForSeconds(.025f);
                for (int k = 0; k < 6; k++)
                {
                    gameObject.GetComponentsInChildren<SpriteRenderer>()[k].color = colors[k];
                }
                yield return new WaitForSeconds(.025f);
            }
        }
       
        yield return new WaitForSeconds(.25f);
        power = 0;
        
    }


}
