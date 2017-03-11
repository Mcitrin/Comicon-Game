using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

    InputMan input;
    float distToGround;

    public int PlayerNumber;
    public GameObject arrow;
    public Vector3 angle;
    public float power;
    public float chargeTime;
    public GameObject ball;

    // Use this for initialization
    void Awake()
    {
        input = GameManager.gameManager.GetComponent<InputMan>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;
    }
    
    // Update is called once per frame
    void Update () {
        if (PlayerNumber == 1)
            Input1();
        else
            Input2();
	}

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void Input1()
    {
        if (input.Jump1() && IsGrounded())
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 7.5f, 0);//7.5f;
        }

        Vector3 curVel = this.GetComponent<Rigidbody>().velocity;
        curVel.x = input.Move1() * 9.0f; // max speed = 5
        this.GetComponent<Rigidbody>().velocity = curVel;

        if (input.Aim1() != Vector2.zero)
        {
            angle = input.Aim1();
        }





        arrow.transform.position = angle + this.transform.position;


    }
    void Input2()
    {
        if (input.Jump2() && IsGrounded())
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 7.5f, 0);//7.5f;
        }

        Vector3 curVel = this.GetComponent<Rigidbody>().velocity;
        curVel.x = input.Move2() * 9.0f; // max speed = 5
        this.GetComponent<Rigidbody>().velocity = curVel;

        if (input.Aim2() != Vector2.zero)
        {
            angle = input.Aim2();
        }





        arrow.transform.position = angle + this.transform.position;


    }
}
