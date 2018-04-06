using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour
{


    // refrence to the inputmanager script attached to the singelton gameManager object
    public InputMan inputMan;
    // the players nubmer 1 or 2
    public int playerNumber;

    // werether or not your currently charging a shot
    bool chargeingHit = false;

    bool justHitBall = false;

    // the last place we thought the ball was gonna land
    float lastBallLandingPoint;

    // the angle your aming at to be passed to the character controller 
    public Vector2 angle;
    // the angle to lerp to
    public Vector2 angleTo = Vector3.zero;

    public Vector2 tmp;

    // handel to the character controller delegate
    CharacterController characterController;

    // handel to the animation controller delegate
    public AnimationController animationController;

    // handel to the apperance delegate (changes sprite colors for cusimization)
    public Appearance appearance;

    public AimReticle reticle;

    BallV2 ball;

    public enum AIState
    {
        IDLE,
        SEEKBALL,
        SERVING
    }

    public AIState aiState = AIState.IDLE;

    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterController.playerNum = playerNumber;
        ball = GameManager.gameManager.ball;
    }

    // Update is called once per frame
    void Update()
    {

        // if the games not paused recive input and allow animation
        if (!GameManager.paused)
        {
            Animate();
            Aiming();
            Move();
            ManageState();

            if (animationController.Animator.speed < 1)
                animationController.Animator.speed = 1;
        }
        else
        {
            if (animationController.Animator.speed > 0)
                animationController.Animator.speed = 0;
        }


        Aiming();
        Move();

    }

    void ManageState()
    {
        if (aiState == AIState.IDLE)
        {
            if (ShouldSeekBall())
            {
                aiState = AIState.SEEKBALL;
            }

        }
        if (aiState == AIState.SEEKBALL)
        {
            if(!ShouldSeekBall())
            {
                aiState = AIState.IDLE;
            }
        }
    }
    void Animate()
    {
        if (characterController.hitting && animationController.Animator.GetCurrentAnimatorStateInfo(1).IsTag("Hit"))
        {
            if (animationController.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime % 1 > .80f)
            {
                characterController.hitting = false;
            }
        }
    }
    void Aiming()
    {
        if(ball.landingPoint != lastBallLandingPoint)
        {
            angleTo = GetAimVector();
            lastBallLandingPoint = ball.landingPoint;
        }

        //lock to front arc
        //if (playerNumber == 1)
        //{
        //    if (angleTo.x < 0 && angleTo.y >= 0)
        //        angleTo = new Vector3(0, 1);
        //    if (angleTo.x < .3f && angleTo.y <= 0)
        //        angleTo = new Vector3(.3f, -1);
        //}
        //if (playerNumber == 2)
        //{
        //    if (angleTo.x > 0 && angleTo.y >= 0)
        //        angleTo = new Vector3(0, 1);
        //    if (angleTo.x > .3f && angleTo.y <= 0)
        //        angleTo = new Vector3(.3f, -1);
        //}

        float xPos = Linear(angle.x, angleTo.x, Time.deltaTime);
        float yPos = Linear(angle.y, angleTo.y, Time.deltaTime);
        angle = new Vector2(xPos, yPos).normalized;

        // pass our angel to the character controller
        characterController.Aim(angle.normalized);

        // hit the ball
        if (ShouldhitBall())
        {
            characterController.setHitMagnitude(GitHitMag());
            animationController.hitBall(GitHitMag() == 2);
            characterController.hitting = true;
            justHitBall = true;
        }

        if(Vector2.Distance(ball.transform.position, transform.position) > ball.radious + characterController.handRadious)
        {
            justHitBall = false;
        }
    }
    int GitHitMag()
    {
        float dist2Net = Mathf.Abs(GameManager.gameManager.net.transform.position.x - transform.position.x);
        int pow = 0;
        if (dist2Net <= GameManager.gameManager.courtSize * .5f)
        {
            pow = GameManager.gameManager.normHit;
        }
        else
        {
            pow = GameManager.gameManager.hardHit;
        }
        return pow;
    }
    Vector2 GetAimVector()
    {
        float landingPoint = 0;

        // get landingPoint
        if(playerNumber == 2)
        {
            landingPoint = Random.Range(GameManager.gameManager.left.position.x + ball.radious,
                GameManager.gameManager.net.position.x - ball.radious);
        }
        else if(playerNumber == 1)
        {
            landingPoint = Random.Range(GameManager.gameManager.net.position.x + ball.radious,
               GameManager.gameManager.right.position.x - ball.radious);
        }

        float dx = landingPoint - ball.landingPoint;
        Debug.Log(dx);
        float T = 3 + Random.value;
        Debug.Log(T);
        int Z = GitHitMag();
        Debug.Log(Z);
        float V0x = dx / T;
        Debug.Log(V0x);
        float V0y = Mathf.Sqrt((Z * Z) - (V0x * V0x));
        Debug.Log(V0y);

        tmp = new Vector2(landingPoint, 0);

        return new Vector2(V0x,V0y);
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * 10);
    }

    void Move()
    {

        int direction2Ball = (int)Mathf.Sign(characterController.hand.position.x - ball.landingPoint) * -1;


        if (aiState == AIState.SEEKBALL)
        {
            animationController.speed = Mathf.Abs(direction2Ball);
            characterController.Move(direction2Ball, false);
        }
        else
        {
            animationController.speed = 0;
        }
    }


    bool ShouldhitBall()
    {
        if (Vector2.Distance(ball.transform.position, transform.position) <= ball.radious + characterController.handRadious + 3f
            && !justHitBall && Mathf.Abs(GameManager.gameManager.net.transform.position.x - ball.landingPoint) > GameManager.gameManager.courtSize)
            return true;

        return false;
    }


    bool ShouldSeekBall()
    {
        // if the balls not in play or were standing where the ball will land
        if (ball.bState != BallV2.BallState.InPlay || Mathf.Abs(ball.landingPoint - characterController.hand.position.x) <= ball.radious)
            return false;

        // if the ball is out of bounds or will land out of bounds
        if (Mathf.Abs(GameManager.gameManager.net.transform.position.x - ball.landingPoint) > GameManager.gameManager.courtSize
            || ball.CourtSide == 'O')
            return false;

        // if the balls on the other side of the court or moveing in that direction
        if (playerNumber == 1)
        {
            if (ball.V.x > 0 || ball.CourtSide == 'R')
                return false;
        }

        if (playerNumber == 2)
        {
            if (ball.V.x < 0 || ball.CourtSide == 'L')
                return false;
        }

        // if the ball is held
        if (ball.bState == BallV2.BallState.Held)
            return false;

        return true;
    }
}
