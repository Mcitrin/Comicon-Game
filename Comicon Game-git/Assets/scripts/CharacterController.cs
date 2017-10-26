using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{

    // put these somewhere else in future
    // these are empy transforms i use to keep the player inside the court
    // im meaning to move them to some class that would contatin information about the level later, but fo4r right now im just testing them
    public Transform left;
    public Transform right;
    public Transform net;
    public Transform ground;

    float courtSize;

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
    public bool jumping;

    Rigidbody2D rigidbody;

    // this ditance the arrow gameoject can be from you
    int arrowDistance = 2;

    float fallDuration = .5f;
    float jump1Duration = .5f;
    float jump2Duration = .5f;

    float maxJumpHeight = 7.5f; //h

    float yV0;
    float G;


    float xVelocity;
    float yVelocity;

    float maxVelocity = .15f;//.15f;

    bool moving;
    bool diveing;

    Vector3 diveDestination;
    Vector3 diveStart;

    float drag = .1f;

    // the size of your collsion box in the x dimension 
    float xBounds;
    float yBounds;

    public Lerper lerper;

    // Use this for initialization
    void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        xBounds = GetComponent<BoxCollider2D>().bounds.extents.x;
        yBounds = GetComponent<BoxCollider2D>().bounds.extents.y;

        gameObject.AddComponent<Lerper>();
        lerper = GetComponent<Lerper>();

        grounded = true;
        courtSize = net.position.x - left.position.x;

        yV0 = (2 * maxJumpHeight) / jump1Duration;
        G = -2 * maxJumpHeight / Mathf.Pow(jump1Duration, 2);
    }

    // Update is called once per frame
    void Update()
    {


        // used to vissualy see the size of my collision box
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.left * xBounds, Color.red);
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.right * xBounds, Color.red);



        // pause check
        if (!GameManager.paused)
        {
            // if the games not paused do this
            //if (rigidbody.isKinematic)
            //rigidbody.isKinematic = false;

            // allows the arrow to move
            arrow.transform.position = angle * arrowDistance + this.transform.position;


            if (transform.position.y < ground.position.y + yBounds)
            {
                //transform.position = new Vector3(transform.position.x, ground.position.y + distToGround, 0);
            }

            if (diveing)
            {
                //transform.position += new Vector3(DontCrossBorder(GetDiveDirection().x, "X"), DontCrossBorder(GetDiveDirection().y,"Y"),0);
                float xPos = EaseOutCubic(transform.position.x, diveDestination.x, Time.deltaTime);//Linear(transform.position.x, diveDestination.x,Time.deltaTime);
                float yPos = EaseOutCubic(transform.position.y, diveDestination.y, Time.deltaTime);//Linear(transform.position.y, diveDestination.y,Time.deltaTime);
                Debug.Log(new Vector3(xPos, yPos, 0));
                transform.position = new Vector3(xPos, yPos, 0);

            }
            else
            {
                if (!grounded) // jumping
                {
                    float pos = 0;
                    float g = G;

                    if (yVelocity > 0 || jumping)
                    {
                        pos += yVelocity * Time.deltaTime + (g * .5f) * (Mathf.Pow(Time.deltaTime, 2));
                    }
                    else if(yVelocity < 0 || !jumping)
                    {
                        pos += yVelocity * Time.deltaTime + (g * .5f) * (Mathf.Pow(Time.deltaTime, 2));
                    }

                    //pos += yVelocity * Time.deltaTime + (g * .5f) * (Mathf.Pow(Time.deltaTime, 2));
                    yVelocity += g * Time.deltaTime;

                    pos = DontCrossBorder(pos, "Y");

                    transform.position += new Vector3(0, pos, 0);

                    IsGrounded();
                }
                xVelocity = DontCrossBorder(xVelocity, "X");
                    transform.position += new Vector3(xVelocity, 0, 0);
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
        }
        else // end pause check
        {
            // if games is paused set kinimatic to true and zero out velocity
            //if (!rigidbody.isKinematic)
            //rigidbody.isKinematic = true;
            //rigidbody.velocity = Vector2.zero;
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
                        xVelocity += (0.25f * Time.deltaTime) * direction * multiplier;
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

    public void Jump(string height)
    {
            grounded = false;
            jumping = true;

            if (height == "low")
            {
                yVelocity = yV0*.5f;
            }
            if (height == "high")
            {
                yVelocity = yV0;
            }
    }
    public void Jump2()
    {
            yVelocity = yV0;
            jumping = true;
    }

    public void ApplyGravity()
    {
        if (!lerper.lerping && !grounded)
        {
            lerper.SetUpLerp(transform.position.y, ground.position.y + yBounds, fallDuration);
        }
    }

    // are we touching the ground
    public bool IsGrounded()
    {
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y - yBounds), Color.red);
        if (transform.position.y - yBounds <= ground.position.y)
        {
            grounded = true;
            jumping = false;
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

    public IEnumerator Dive(float length)
    {
        diveing = true;
        diveDestination = GetDiveDirection();
        diveStart = transform.position;
        Debug.Log(diveDestination);
        Debug.Log(diveStart);
        yVelocity = 0;
        yield return new WaitForSeconds(length);
        diveing = false;
    }

    Vector3 GetDiveDirection()
    {
        Vector3 dir = Vector3.zero;

        if (grounded)
        {
            if ((transform.position.x + xBounds) + (courtSize / 2) > net.position.x)
            {
                dir = Vector3.right * (net.position.x - (transform.position.x + xBounds));
            }
            else
            {
                dir = Vector3.right * (courtSize / 2);
            }
        }
        else
        {
            dir = new Vector3(1, -1, 0);
        }

        return transform.position + dir;
    }

    public static float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    bool touchingSides()
    {
        if (xVelocity < 0)// moving left
        {
            if (playerNum == 1)
            {
                if (transform.position.x - xBounds <= left.position.x)
                {
                    transform.position = new Vector3(left.position.x + xBounds, transform.position.y,0);
                    xVelocity = 0;
                    return true;
                }
            }
            if (playerNum == 2)
            {
                if (transform.position.x - xBounds <= net.position.x)
                {
                    transform.position = new Vector3(net.position.x + xBounds, transform.position.y, 0);
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
                    transform.position = new Vector3(net.position.x - xBounds, transform.position.y, 0);
                    xVelocity = 0;
                    return true;
                }
            }
            if (playerNum == 2)
            {
                if (transform.position.x + xBounds >= right.position.x)
                {
                    transform.position = new Vector3(right.position.x - xBounds, transform.position.y, 0);
                    xVelocity = 0;
                    return true;
                }
            }
        }
        return false;
    }

    float DontCrossBorder(float moveDelta, string coordinate)
    {
        if (coordinate == "X")
        {
            if (xVelocity < 0)// moving left
            {
                if (playerNum == 1)
                {
                    if (transform.position.x - xBounds < left.position.x)
                    {

                        return (int)((transform.position.x - xBounds) - left.position.x);
                    }
                }
                if (playerNum == 2)
                {
                    if (transform.position.x - xBounds <= net.position.x)
                    {
                        return (int)((transform.position.x - xBounds) - net.position.x);
                    }
                }
            }
            if (xVelocity > 0)// moving right
            {
                if (playerNum == 1)
                {
                    if ((transform.position.x + xBounds) + moveDelta > net.position.x)
                    {;
                        return (int)((transform.position.x + xBounds) - net.position.x);
                    }
                }
                if (playerNum == 2)
                {
                    if ((transform.position.x + xBounds) + moveDelta > right.position.x)
                    {
                        return (int)((transform.position.x + xBounds) - right.position.x);
                    }
                }
            }
        }
        else if (coordinate == "Y")
        {
            if ((transform.position.y - yBounds) + moveDelta < ground.position.y)
            {
                Debug.Log((transform.position.y - yBounds) - ground.position.y);
                return -((transform.position.y - yBounds) - ground.position.y);
            }
        }
        return moveDelta;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            CharacterController Player = collision.gameObject.GetComponentInParent<CharacterController>();

            if (Player.power == 1 || Player.power == 2)
            {
                Debug.Log(Player.gameObject.name);
                this.gameObject.GetComponentInParent<Rigidbody>().AddForce((transform.position - collision.transform.position) * 100, ForceMode.Impulse);
            }
        }
    }
}
