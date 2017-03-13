using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

    InputMan inputMan;
    float distToGround;

    public int PlayerNumber;
    public GameObject arrow;
    public Vector3 angle;
    public float power;
    public float chargeTime;
    public Ball ball;
    float distance = 2;

    // Use this for initialization
    void Awake()
    {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;
    }
    
    // Update is called once per frame
    void Update () {
            Input();

    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void Input()
    {
        if (inputMan.Jump(PlayerNumber) && IsGrounded())
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 7.5f, 0);//7.5f;
        }

        Vector3 curVel = this.GetComponent<Rigidbody>().velocity;
        curVel.x = inputMan.Move(PlayerNumber) * 9.0f; // max speed = 5
        this.GetComponent<Rigidbody>().velocity = curVel;

        if (inputMan.Aim(PlayerNumber) != Vector2.zero)
        {
            angle = inputMan.Aim(PlayerNumber);
        }





        arrow.transform.position = angle + this.transform.position;
        CalcPower();

    }


    void CalcPower()
    {
        if (inputMan.Charge(PlayerNumber))
        {
            if (power == 0)
                power = Time.time;
        }

        if (!inputMan.Charge(PlayerNumber))
        {
            if (power != 0)
            {
                chargeTime = (int)(Time.time - power);
                if (chargeTime >= 1)
                {
                    power = 2;
                    StartCoroutine(Flash());
                }
                else
                {
                    power = 1;
                }

                if (Vector3.Distance(new Vector2(this.transform.position.x,this.transform.position.y)
                    ,ball.transform.position) <= distance)
                {
                    ball.HitBall((int)power, angle , PlayerNumber);
                }

                chargeTime = 0;
                power = 0;
            }
        }
    }
  
    IEnumerator Flash()
    {
        Color color = this.gameObject.GetComponent<SpriteRenderer>().color;
        for (int i = 0; i < 8; i++)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(.025f);
            gameObject.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(.025f);
        }
    }


}
