using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    // put these somewhere else in future
    public Transform left;
    public Transform right;
    public Transform net;



    public int power;
    public Vector3 angle;
    public GameObject arrow;
    public GameObject hand;
    Rigidbody2D rigidbody;
    int arrowDistance = 2;

    float jumpVel = 15.5f;
    float fallMultiplier = 4.5f;

    Vector2 velocity;
    float maxVelocity = 7.5f;
    bool stop = false;
    float xBounds;

    // Use this for initialization
    void OnEnable () {
        rigidbody = GetComponent<Rigidbody2D>();
        xBounds = GetComponent<BoxCollider2D>().bounds.extents.x + 1;
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(left.position);
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.left * xBounds, Color.red);
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.right * xBounds, Color.red);

        // pause check
        if (!GameManager.paused)
        {
            if (rigidbody.isKinematic)
            rigidbody.isKinematic = false;
            arrow.transform.position = angle * arrowDistance + this.transform.position;
            




            // set x velocity to physics velocity in x 
            velocity.x = rigidbody.velocity.x;
            rigidbody.velocity = new Vector2(velocity.x, velocity.y);
        }
        else
        {
            if (!rigidbody.isKinematic)
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
        }
    }

    public void Aim(Vector3 arrowAngle)
    {
        angle = arrowAngle; 
    }

    public void Move(float vel, int playerNum)
    {



        // clean this up if you can think of a better way


        if (vel < 0)// moving left
        {
            if (playerNum == 1)
            {
                if (transform.position.x - xBounds <= left.position.x)
                {
                    rigidbody.velocity = new Vector2(0, velocity.y);
                    return;
                }
            }
            if (playerNum == 2)
            {
                if (transform.position.x - xBounds <= net.position.x)
                {
                    rigidbody.velocity = new Vector2(0, velocity.y);
                    return;
                }
            }

        }

        if (vel > 0)// moving right
        {
            if (playerNum == 1)
            {
                if (transform.position.x + xBounds >= net.position.x)
                {
                    rigidbody.velocity = new Vector2(0, velocity.y);
                    return;
                }
            }
            if (playerNum == 2)
            {
                if (transform.position.x + xBounds >= right.position.x)
                {
                    rigidbody.velocity = new Vector2(0, velocity.y);
                    return;
                }
            }

        }

        if (Mathf.Abs(rigidbody.velocity.x) < maxVelocity)
            rigidbody.AddForce(new Vector2(vel, 0.0f));

    }

    public void jump(bool jump, bool grounded)
    {

        if (grounded)
        {
            velocity.y = 0;
        }
        if (jump)
        {
            velocity.y = jumpVel;
        }
       

        // jump calculations
        if (rigidbody.velocity.y < 0 && !grounded)
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1 *  1.5f) * Time.deltaTime;
        }
        else if (rigidbody.velocity.y > 0)
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            
        }
    }

    public IEnumerator setPower(int pow)
    {
        power = pow;
        yield return new WaitForSeconds(.25f);
        power = 0;
    }



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            CharacterController Player = collision.gameObject.GetComponentInParent<CharacterController>();
            
            if (Player.power == 1 || Player.power == 2)
            {
                Debug.Log(Player.gameObject.name);
                this.gameObject.GetComponentInParent<Rigidbody>().AddForce((transform.position - collision.transform.position) * 100,ForceMode.Impulse);
            }
        }
    }
}
