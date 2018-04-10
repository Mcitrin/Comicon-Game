using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{

    float courtSize;
    public int playerNum;

    // the power that the player hits the ball with 1 for a normal hit 2 for a power hit
    public int power;
    // the angle that your aiming, the ball class uses this to determin where to hit the ball
    public Vector3 angle;
    // a gameobject with a sprite used to visualize where the players aiming
    public GameObject arrow;
    // this ditance the arrow gameoject can be from you (it radious)
    float arrowDistance = 4f;

    // and empty transform with a collsion box set to trigger. fallows the hand of the sprite used for colliding with the ball
    public Transform hand;
    public float handRadious;
    public bool hitting;
    
    public bool grounded = true;
    public bool falling = false;
    float jump1Duration = 1;
    float maxJumpHeight = 12.5f; //h

    public float yV0;
    public float G;

    public float GMultiplier;
    public float xVelocity;
    float yVelocity;

    float maxVelocity = .15f;//.15f;
    bool moving;
    float drag = .1f;

    // the size of your collsion box
    float xBounds;
    float yBounds;
    float yOffset;

    // Evants
    public delegate void JumpApexReached(int playerNumber);
    public event JumpApexReached jumpApexReached;

    // Use this for initialization
    public void Init()
    {
        xBounds = GetComponent<BoxCollider2D>().bounds.extents.x;
        yBounds = GetComponent<BoxCollider2D>().bounds.extents.y;
        yOffset = GetComponent<BoxCollider2D>().offset.y;
        handRadious = hand.GetComponent<CircleCollider2D>().radius;
        
        //handYBounds = hand.GetComponent<BoxCollider2D>().bounds.extents.y;
        //courtSize = GameManager.gameManager.net.position.x - GameManager.gameManager.left.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // used to vissualy see the size of my collision box
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.left * xBounds, Color.red);
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y) + Vector2.right * xBounds, Color.red);
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, (transform.position.y + yOffset) - yBounds), Color.red);

        // pause check
        if (!GameManager.paused)
        {

            // allows the arrow to move
            arrow.transform.position = angle * arrowDistance + this.transform.position;
            

            if (!grounded) // jumping
                {
                float yPos = 0;
                float g = G;// * GMultiplier;
                if (yVelocity > 0)
                    {
                        yPos += yVelocity * Time.deltaTime + (g * .5f) * (Mathf.Pow(Time.deltaTime, 2));
                    }
                    else if(yVelocity < 0)
                    {
                        yPos += yVelocity * Time.deltaTime + (g* .5f) * (Mathf.Pow(Time.deltaTime, 2));
                        if(!falling)
                        {
                            falling = true;
                            if (jumpApexReached != null)
                                jumpApexReached(playerNum);
                        }
                    }
                
                yVelocity += g * Time.deltaTime;
                transform.position += new Vector3(0, yPos, 0);
                IsGrounded();
                }

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

            ClampPosition();
        }
        else // end pause check
        {

        }
    }

    public void Aim(Vector2 arrowAngle)
    {
        // gets the angle from the player controller object
        angle = arrowAngle.normalized;
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

    public void Jump(float jumpPercentage)
    {
        //Debug.Log(jumpPercentage);
        grounded = false;

        float jumpHeight = maxJumpHeight * jumpPercentage;

        yV0 = (2 * jumpHeight) / (jump1Duration*jumpPercentage);
        G = (-2 * jumpHeight) / Mathf.Pow(jump1Duration * jumpPercentage, 2);
        yVelocity = yV0;
    }

    // are we touching the ground
    public bool IsGrounded()
    {
        if ((transform.position.y + yOffset) - yBounds <= GameManager.gameManager.ground.position.y)
        {
            grounded = true;
            falling = false;
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    public void setHitMagnitude(int pow)
    {
        power = pow;   
    }
    public Vector3 getHitVector()
    {
        return new Vector3(angle.x,angle.y,power);
    }

    void ClampPosition()
    {

        float yClamp = Mathf.Clamp(transform.position.y, GameManager.gameManager.ground.position.y + (yBounds - yOffset), 1000);
        float xClamp = 0;


        if (playerNum == 1)
        {
             xClamp = Mathf.Clamp(transform.position.x, GameManager.gameManager.left.position.x + xBounds, 
                                                        GameManager.gameManager.net.position.x - xBounds);
        }
        else if(playerNum == 2)
        {
             xClamp = Mathf.Clamp(transform.position.x,GameManager.gameManager.net.position.x + xBounds,
                                                       GameManager.gameManager.right.position.x - xBounds);


        }
        transform.position = new Vector3(xClamp, yClamp);
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * 10);
    }
}
