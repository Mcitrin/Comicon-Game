using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    // put these somewhere else in future
    // these are empy transforms i use to keep the player inside the court
    // im meaning to move them to some class that would contatin information about the level later, but fo4r right now im just testing them
    public Transform left;
    public Transform right;
    public Transform net;
    public Transform ground;

    public int playerNum;

    // the power that the player hits the ball with 1 for a normal hit 2 for a power hit
    public int power;
    // the angle that your aiming, the ball class uses this to determin where to hit the ball
    public Vector3 angle;
    // a gameobject with a sprite used to visualize where the players aiming
    public GameObject arrow;
    // and empty transform with a collsion box set to trigger. fallows the hand of the sprite used for colliding with the ball
    public GameObject hand;

    public bool grounded;
   
    Rigidbody2D rigidbody;

    // this ditance the arrow gameoject can be from you
    int arrowDistance = 2;
   
    float fallDuration = .5f;
    float jump1Duration = 1;
    float jump2Duration = .5f;

    public float maxJumpHeight = 10f;

    float xVelocity;

    float maxVelocity = .15f;

    bool moving;

    float drag = .1f;

    // the size of your collsion box in the x dimension 
    float xBounds;
    float distToGround;

    public Lerper lerper;

    public

    // Use this for initialization
    void OnEnable () {
        rigidbody = GetComponent<Rigidbody2D>();
        xBounds = GetComponent<BoxCollider2D>().bounds.extents.x + 1;
        distToGround = GetComponent<BoxCollider2D>().bounds.extents.y;

        gameObject.AddComponent<Lerper>();
        lerper = GetComponent<Lerper>();

        grounded = true;
    }
	
	// Update is called once per frame
	void Update () {


        // used to vissualy see the size of my collision box
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.left * xBounds, Color.red);
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.right * xBounds, Color.red);

        
        
        // pause check
        if (!GameManager.paused)
        {
            // if the games not paused do this
            if (rigidbody.isKinematic)
            rigidbody.isKinematic = false;

            // allows the arrow to move
            arrow.transform.position = angle * arrowDistance + this.transform.position;

            touchingSides();
            
            transform.position += new Vector3(xVelocity,0,0);

            if (!moving)
            {
                xVelocity -= xVelocity * drag;
                if (Mathf.Abs(xVelocity) < 0.001f)
                {
                    xVelocity = 0f; // stop
                }
            }
            moving = false;
        }
        else
        {
            // if games is paused set kinimatic to true and zero out velocity
            if (!rigidbody.isKinematic)
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector2.zero;
        }
    }

    public void Aim(Vector2 arrowAngle)
    {
        // gets the angle from the player controller object
        angle = arrowAngle; 
    }

    public void Move(int direction, bool moveSlow)
    {
        if (!GameManager.paused)
        {
            if (direction != 0)
            {
                if (Mathf.Abs(xVelocity) < maxVelocity)
                {
                    float multiplier = 2f * Mathf.Clamp01(1 - Mathf.Pow(Mathf.Abs(xVelocity) / maxVelocity, 2f));
                    if (moveSlow)
                    {
                        xVelocity += (0.45f * Time.deltaTime) * direction * multiplier;
                    }
                    else
                    {
                        xVelocity += (0.75f * Time.deltaTime) * direction * multiplier;
                    }
                    moving = true; // moved this frame
                }
            }
        }
    }

    public void Jump()
    {
        grounded = false;
        float jumpHeight = ground.position.y + maxJumpHeight + 1;

        if (!lerper.lerping)
        {
            lerper.SetUpLerp(transform.position.y, jumpHeight, jump1Duration);
        }
    }
    public void Jump2()
    {
        float jumpHeight = transform.position.y + (maxJumpHeight * .3f);
        lerper.SetUpLerp(transform.position.y, jumpHeight, jump2Duration);
    }

    public void ApplyGravity()
    {
        if (!lerper.lerping && !grounded)
        {
            lerper.SetUpLerp(transform.position.y, ground.position.y + distToGround, fallDuration);
        }
    }

    // are we touching the ground
    public bool IsGrounded()
    {
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y - distToGround), Color.red);
        if (transform.position.y - distToGround <= ground.position.y)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    public IEnumerator setPower(int pow)
    {
        power = pow;
        // wait a 4th of a second befor you can hit again
        yield return new WaitForSeconds(.25f);
        power = 0;
    }

    public void Dive(float length)
    {
            //if(grounded)
            //rigidbody.AddForce(Vector2.right * 20);
            //else
            //rigidbody.AddForce(new Vector2(1,-1) * 5);
    }

    bool touchingSides()
    {
        if (xVelocity < 0)// moving left
        {
            if (playerNum == 1)
            {
                if (transform.position.x - xBounds <= left.position.x)
                {
                    rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                    xVelocity = 0;
                    return true;
                }
            }
            if (playerNum == 2)
            {
                if (transform.position.x - xBounds <= net.position.x)
                {
                    rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                    xVelocity = 0;
                    return true;
                }
            }
        }
        // if your moveing right
        if (xVelocity > 0)// moving right
        {
            if (playerNum == 1)
            {
                if (transform.position.x + xBounds >= net.position.x)
                {
                    rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                    xVelocity = 0;
                    return true;
                }
            }
            if (playerNum == 2)
            {
                if (transform.position.x + xBounds >= right.position.x)
                {
                    rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                    xVelocity = 0;
                    return true;
                }
            }
        }
        return false;
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
