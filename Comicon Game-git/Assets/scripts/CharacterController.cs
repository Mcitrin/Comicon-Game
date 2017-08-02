using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    // put these somewhere else in future
    // these are empy transforms i use to keep the player inside the court
    // im meaning to move them to some class that would contatin information about the level later, but fo4r right now im just testing them
    public Transform left;
    public Transform right;
    public Transform net;

    
    // the power that the player hits the ball with 1 for a normal hit 2 for a power hit
    public int power;
    // the angle that your aiming, the ball class uses this to determin where to hit the ball
    public Vector3 angle;
    // a gameobject with a sprite used to visualize where the players aiming
    public GameObject arrow;
    // and empty transform with a collsion box set to trigger. fallows the hand of the sprite used for colliding with the ball
    public GameObject hand;
   
    Rigidbody2D rigidbody;
    // this ditance the arrow gameoject can be from you
    int arrowDistance = 2;

    float drag = .05f;
    float diveDuration = .5f;

    // the velocity added to your gamebojcet when you jump
    float jumpVel = 15.5f;
    
    // gravity is multiplied by this and added to your gameobject when your jumping
    float fallMultiplier = 4.5f;

    // this vector is added to your ridgidBody.velocity  every update
    Vector2 velocity;

    // this limits you velocity 
    float maxVelocity = 7.5f;
    
    // the size of your collsion box in the x dimension 
    float xBounds;

    // Use this for initialization
    void OnEnable () {
        rigidbody = GetComponent<Rigidbody2D>();
        xBounds = GetComponent<BoxCollider2D>().bounds.extents.x + 1;
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





            // set x velocity to physics velocity in x 
            velocity.x = rigidbody.velocity.x - (rigidbody.velocity.x * drag);

            // set ridgid body.velocty to the velocity we calculated
            rigidbody.velocity = new Vector2(velocity.x, velocity.y);
        }
        else
        {
            // if games is paused set kinimatic to true and zero out velocity
            if (!rigidbody.isKinematic)
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
        }
    }

    public void Aim(Vector3 arrowAngle)
    {
        // gets the angle from the player controller object
        angle = arrowAngle; 
    }

    public void Move(float vel, int playerNum)
    {
        // takes in velocity and the player number from player controller


        // theirs probably a better  more efficant way to do all this
        // basically all this dose is limit the players movement if their touching the left ro right boundarys or the net
        // if you player 1 ie on the left side of the net you stuck between the net and the left boundary
        // else if your player 2 your stuck between the net and the right boundary
        

        // if your moving left
        if (vel < 0 || rigidbody.velocity.x < 0)// moving left
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

        // if your moveing right
        if (vel > 0 || rigidbody.velocity.x > 0)// moving right
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

        // only add force if your below the max velocity
        if (Mathf.Abs(rigidbody.velocity.x) < maxVelocity)
            rigidbody.AddForce(new Vector2(vel, 0.0f));

    }

    public void Jump()
    {
        velocity.y = jumpVel;
    }

    public void ApplyGravity(bool grounded)
    {
        if (grounded)
        {
            velocity.y = 0;

        }
        // if were moving down add more downward force
        if (rigidbody.velocity.y < 0)
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1 * 1.5f) * Time.deltaTime;
        }
        // els if were moving up add less downward force
        else if (rigidbody.velocity.y > 0)
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

        }
    }

    public IEnumerator setPower(int pow)
    {
        // takes in the power of you hit from player controller , this info is pased to the ball when your hand colides with it

        power = pow;
        // wait a 4th of a second befor you can hit again
        yield return new WaitForSeconds(.25f);
        power = 0;
    }

    public void Dive()
    {
        rigidbody.velocity = new Vector2(0, velocity.y);
        float time = 1.0f;
        while (time > 0)
        {
            if (transform.position.x + xBounds >= net.position.x)
            {
                rigidbody.velocity = new Vector2(0, velocity.y);
                return;
            }

            time -= Time.deltaTime;
            rigidbody.AddForce(Vector2.right * 20);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // this is a new machanic i havnt yet tested, its suposed to allow you to smack the other player if you hit them
        // esentially what im saying is if the collider on your hand collides with the other player move that player back


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
