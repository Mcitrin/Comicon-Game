using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{

    InputMan inputMan;
    AnimationManager animationManager;
    float distToGround;

    public Animator uperBody;
    public Animator lowerBody;
    public AnimationClip smack;
    public AnimationClip set;
    public int PlayerNumber;
    public GameObject arrow;
    public SpriteRenderer Hair;
    public SpriteRenderer Shorts;
    public SpriteRenderer Stripe;
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
        Input();
        Animate();
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.2f);
    }

    void Input()
    {
        if (inputMan.Jump(PlayerNumber) && IsGrounded())
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 10.0f, 0);//7.5f;
        }

        Vector3 curVel = this.GetComponent<Rigidbody>().velocity;
        if (!IsGrounded() || chargeing)
        {
            curVel.x = inputMan.Move(PlayerNumber) * 2.5f;
        }
        else
        {
            curVel.x = inputMan.Move(PlayerNumber) * 5.0f;
        }

        this.GetComponent<Rigidbody>().velocity = curVel;

        if (inputMan.Aim(PlayerNumber) != Vector2.zero)
        {
            angle = inputMan.Aim(PlayerNumber);
        }
        
        arrow.transform.position = angle*1.5f + this.transform.position;
        HitBall();
    }

    void Animate()
    {
        lowerBody.SetBool("Jump", !IsGrounded());
        uperBody.SetBool("Jump", !IsGrounded());

        if (inputMan.Move(PlayerNumber) < 0)
        {
            lowerBody.SetBool("Backward", true);
            lowerBody.SetBool("Forward", false);
            lowerBody.SetBool("Still", false);
            // uper
            uperBody.SetBool("Move", true);
            uperBody.SetBool("Still", false);
        }
        else if(inputMan.Move(PlayerNumber) > 0)
        {
            lowerBody.SetBool("Backward", false);
            lowerBody.SetBool("Forward", true);
            lowerBody.SetBool("Still", false);
            // uper
            uperBody.SetBool("Move", true);
            uperBody.SetBool("Still", false);
        }
        else
        {
            lowerBody.SetBool("Backward", false);
            lowerBody.SetBool("Forward", false);
            lowerBody.SetBool("Still", true);
            // uper
            uperBody.SetBool("Move", false);
            uperBody.SetBool("Still", true);
        }
    }

    void HitBall()
    {
        if (inputMan.Charge(PlayerNumber))
        {
            if (!IsGrounded()) // jumping and spikeing
            {
                animationManager.SetBool(uperBody, "Smack", smack.length);
                uperBody.SetBool("Set", false);
                if (Vector3.Distance(new Vector2(this.transform.position.x, this.transform.position.y)
                , ball.transform.position) <= distance || ball.HeldBy == gameObject)
                {
                    ball.HitBall(2, angle, PlayerNumber, true);
                   StartCoroutine(Flash());
                }
            }
            else              // standing and setting
            {
                animationManager.SetBool(uperBody, "Set", set.length);
                uperBody.SetBool("Smack", false);
                if (Vector3.Distance(new Vector2(this.transform.position.x, this.transform.position.y)
                , ball.transform.position) <= distance || ball.HeldBy == gameObject)
                {
                    ball.HitBall(1, angle, PlayerNumber, false);
                }
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
