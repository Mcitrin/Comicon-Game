using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {


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
    void Start () {
        characterController = GetComponent<CharacterController>();
        characterController.playerNum = playerNumber;
        ball = GameManager.gameManager.ball;
	}
	
	// Update is called once per frame
	void Update () {
        
        // if the games not paused recive input and allow animation
        if (!GameManager.paused)
        {
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
        if(aiState == AIState.IDLE)
        {
            if ((playerNumber == 1 && ball.CourtSide == 'L')
                || (playerNumber == 2 && ball.CourtSide == 'R'))
            {
                aiState = AIState.SEEKBALL;
            }
            
        }
        if(aiState == AIState.SEEKBALL)
        {
            if ((playerNumber == 2 && ball.CourtSide == 'L')
               || (playerNumber == 1 && ball.CourtSide == 'R')
               || ball.CourtSide == 'O')
            {
                aiState = AIState.IDLE;
            }
        }
    }

    void Aiming()
    {

    }

    void Move()
    {

        int direction2Ball = (int)Mathf.Sign(transform.position.x - ball.landingPoint) * -1;
        

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
