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
    public SpriteRenderer HairSprite;
    public SpriteRenderer ShortsSprite;
    public SpriteRenderer StripeSprite;
    public Vector3 angle;
    public float power;
    public float chargeTime;
    public Ball ball;

    float distance = 3;

    //bool setting = false;
    bool chargeing = false;
    Color[] colors = new Color[3];

    // Use this for initialization
    void Awake()
    {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
        animationManager = GameManager.gameManager.GetComponent<AnimationManager>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;

        
        colors[0] = gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color;
        colors[1] = gameObject.GetComponentsInChildren<SpriteRenderer>()[1].color;
        colors[2] = gameObject.GetComponentsInChildren<SpriteRenderer>()[2].color;
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        Input();
        
      if(!IsGrounded())
      this.GetComponent<Rigidbody>().velocity += Vector3.down *.2f;

        //Debug.Log((int)inputMan.Move(PlayerNumber));

    }

    bool IsGrounded()
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
            curVel.x = inputMan.Move(PlayerNumber) * 2.5f;
        }
        else
        {
            curVel.x = inputMan.Move(PlayerNumber) * 7.5f;//5
        }

        this.GetComponent<Rigidbody>().velocity = curVel;

        if (inputMan.Aim(PlayerNumber) != Vector2.zero)
        {
            angle = inputMan.Aim(PlayerNumber);
        }
        
        arrow.transform.position = angle*1.5f + this.transform.position;

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
            if (power == 0)
                power = Time.time;
            chargeTime = (Time.time - power);
        }

        if (chargeTime >= .2f)
        {

            chargeing = true;
        }
        else { chargeing = false; }

        if (!inputMan.Charge(PlayerNumber))
        {
            if (power != 0)
            {

                if (chargeTime >= .75)
                {
                    power = 2;
                    StartCoroutine(Flash());
                }
                else
                {
                    power = 1;
                }

              if (Vector3.Distance(new Vector2(this.transform.position.x, this.transform.position.y)
                  , ball.transform.position) <= distance || ball.HeldBy == gameObject)
               {
                   ball.HitBall((int)power, angle, PlayerNumber, !IsGrounded());
               }


                if (!IsGrounded())
                {
                    animator.SetTrigger("Smack");
                }
                else
                {
                    animator.SetTrigger("Set");
                }

               //   if(ball.CollidingPlayer == gameObject)
               // {
               //     ball.HitBall((int)power, angle, PlayerNumber, !IsGrounded());
               // }

                chargeTime = 0;
                power = 0;
            }
        }
    }
    IEnumerator Flash()
    {
        for (int i = 0; i < 8; i++)
        {
            gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color = Color.red;
            gameObject.GetComponentsInChildren<SpriteRenderer>()[1].color = Color.red;
            gameObject.GetComponentsInChildren<SpriteRenderer>()[2].color = Color.red;
            yield return new WaitForSeconds(.025f);
            gameObject.GetComponentsInChildren<SpriteRenderer>()[0].color = colors[0];
            gameObject.GetComponentsInChildren<SpriteRenderer>()[1].color = colors[1];
            gameObject.GetComponentsInChildren<SpriteRenderer>()[2].color = colors[2];
            yield return new WaitForSeconds(.025f);
        }
    }


}
