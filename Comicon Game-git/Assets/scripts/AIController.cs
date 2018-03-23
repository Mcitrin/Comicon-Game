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

    // the angle your aming at to be passed to the character controller 
    Vector2 angle;
    // the angle to lerp to
    Vector2 angleTo = Vector3.zero;

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
            if ((playerNumber == 1 && ball.CourtSide == 'L')
                || (playerNumber == 2 && ball.CourtSide == 'R'))
            {
                aiState = AIState.SEEKBALL;
            }

        }
        if (aiState == AIState.SEEKBALL)
        {
            if ((playerNumber == 2 && ball.CourtSide == 'L')
               || (playerNumber == 1 && ball.CourtSide == 'R')
               || ball.CourtSide == 'O')
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
                characterController.hitting = false;
        }
    }
    void Aiming()
    {
        angleTo = GetAimVector();
        
        // lock to front arc
        if (playerNumber == 1)
        {
            if (angleTo.x < 0 && angleTo.y >= 0)
                angleTo = new Vector3(0, 1);
            if (angleTo.x < .3f && angleTo.y <= 0)
                angleTo = new Vector3(.3f, -1);
        }
        if (playerNumber == 2)
        {
            if (angleTo.x > 0 && angleTo.y >= 0)
                angleTo = new Vector3(0, 1);
            if (angleTo.x > .3f && angleTo.y <= 0)
                angleTo = new Vector3(.3f, -1);
        }

        float xPos = Linear(angle.x, angleTo.x, Time.deltaTime);
        float yPos = Linear(angle.y, angleTo.y, Time.deltaTime);
        angle = new Vector2(xPos, yPos).normalized;
        
        // pass our angel to the character controller
        characterController.Aim(angle.normalized);


        if (Vector2.Distance(ball.transform.position, transform.position) <= ball.radious + characterController.handRadious)
        {
            characterController.setHitMagnitude(GitHitMag());
            animationController.hitBall(GitHitMag() == 2);
            characterController.hitting = true;
        }
    }
    int GitHitMag()
    {
        float dist2Net = Mathf.Abs(GameManager.gameManager.net.transform.position.x - transform.position.x);
        int pow = 0;
        if (dist2Net <= GameManager.gameManager.courtSize * .5f)
        {
            pow = 1;
        }
        else
        {
            pow = 2;
        }
        return pow;
    }
    Vector2 GetAimVector()
    {
        return -ball.V.normalized;
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * 10);
    }

    void Move()
    {

        int direction2Ball = (int)Mathf.Sign(characterController.hand.position.x - ball.landingPoint) * -1;


        if (aiState == AIState.SEEKBALL && ball.bState == BallV2.BallState.InPlay
            && Mathf.Abs(ball.landingPoint - transform.position.x) > ball.radious)
        {
            animationController.speed = Mathf.Abs(direction2Ball);
            characterController.Move(direction2Ball, false);
        }
        else
        {
            animationController.speed = 0;
        }
    }
}
