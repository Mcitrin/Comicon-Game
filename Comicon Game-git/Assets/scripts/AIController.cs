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
    float chargeTime = 0;

    public bool gotServeVector = false;
    float ServeWaitTimer = 0;
    Vector2 ServeWaitRange = new Vector2(1,3); //(min,max)
    float ServeWaitTime = 2;

    //bools to prevent multiple checks
    bool checkingShouldSeek = false;
    bool checkingShouldHit = false;
    bool checkingShouldGetAimVec = false;


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

        ball.pointScored += delegate (int PlayerNumber)
        {
            if (PlayerNumber == playerNumber)
            {
                aiState = AIState.SERVING;
            }
        };


        int direction2Net = (int)Mathf.Sign(transform.position.x - GameManager.gameManager.net.position.x) * -1;
        angle = new Vector2(direction2Net, 1);
        angleTo = angle;
    }

    // Update is called once per frame
    void Update()
    {

        // if the games not paused recive input and allow animation
        if (!GameManager.paused)
        {
            ManageState();
            Animate();
            Aiming();
            Hitting();
            Move();

            if (animationController.Animator.speed < 1)
                animationController.Animator.speed = 1;

            if(ball.bState == BallV2.BallState.Reset && chargeingHit)
            {
                chargeingHit = false;
            }
        }
        else
        {
            if (animationController.Animator.speed > 0)
                animationController.Animator.speed = 0;
        }
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
            if (!checkingShouldSeek)
            {
                checkingShouldSeek = true;
                if (!ShouldSeekBall())
                {
                    aiState = AIState.IDLE;
                }
                checkingShouldSeek = false;
            }
        }

        // redundant check, should only be necessary if game starts with us serving
        if (ball.HeldBy == characterController && aiState != AIState.SERVING)
        {
            aiState = AIState.SERVING;
        }
        else if(ball.HeldBy != characterController && aiState == AIState.SERVING)
        {
            aiState = AIState.IDLE;
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
        animationController.chargeingHit = chargeingHit;
    }
    void Aiming()
    {
        if (chargeingHit)
        {
            chargeTime += Time.deltaTime;
            reticle.Value = Mathf.Clamp(chargeTime, 0, 1);
        }
        else
        {
            reticle.Value = 0;
            chargeTime = 0;
        }

        if (!checkingShouldGetAimVec)
        {
            checkingShouldGetAimVec = true;
            if (ShouldGetAimVector())
                angleTo = GetAimVector();
            checkingShouldGetAimVec = false;
        }

        float xPos = Linear(angle.x, angleTo.x, Time.deltaTime);
        float yPos = Linear(angle.y, angleTo.y, Time.deltaTime);
        angle = new Vector2(xPos, yPos).normalized;

        // pass our angel to the character controller
        characterController.Aim(angle.normalized);
    }
    int GitHitMag()
    {
        float dist2Net;

        if (aiState != AIState.SERVING)
        {
            dist2Net = Mathf.Abs(GameManager.gameManager.net.transform.position.x - ball.landingPoint);
        }
        else
        { 
            dist2Net = Mathf.Abs(GameManager.gameManager.net.transform.position.x - ball.transform.position.x);
        }

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
        float landingPoint = 0;

        // get landingPoint
        if (playerNumber == 2)
        {
            landingPoint = Random.Range(GameManager.gameManager.left.position.x + ball.radious,
                GameManager.gameManager.net.position.x - ball.radious);
        }
        else if (playerNumber == 1)
        {
            landingPoint = Random.Range(GameManager.gameManager.net.position.x + ball.radious,
               GameManager.gameManager.right.position.x - ball.radious);
        }

        // magnitue of hit vector
        float M = GitHitMag();
        //Debug.Log(M);

        if (M == 1) { M = GameManager.gameManager.normHit; }
        else if (M == 2) { M = GameManager.gameManager.hardHit; }

        float Y0 = characterController.hand.position.y + ball.radious;//ball.transform.position.y;

        float DX;
        if (aiState != AIState.SERVING)
        {
            //Debug.Log(landingPoint);
            //Debug.Log(ball.landingPoint);
            DX = landingPoint - ball.landingPoint;
        }
        else
        {
            Debug.Log(landingPoint);
            Debug.Log(ball.transform.position.x);
            DX = landingPoint - ball.transform.position.x;
        }

        float Theta = solve4T2(M, -DX, -Y0);

        float V0x = -(M * Mathf.Cos(Theta));
        float V0y = M * Mathf.Sin(Theta);

        tmp = new Vector2(landingPoint, 0);


        // used to tell if ai should charge a hit
        float ballDistnace2Net = Mathf.Abs(GameManager.gameManager.net.transform.position.x - ball.landingPoint);
        if (M == GameManager.gameManager.hardHit && ballDistnace2Net <= GameManager.gameManager.courtSize)
        {
            chargeingHit = true;
        }

        return new Vector2(V0x, V0y);
    }

    float solve4T2(float M, float DX, float Y0)
    {
        // https://en.wikipedia.org/wiki/Projectile_motion
        float G = 9.8f;
        float root1 = Mathf.Atan(((M * M) + Mathf.Sqrt((M * M * M * M) - G * (G * (DX * DX) + (2.0f * Y0 * (M * M))))) / (G * DX));
        float root2 = Mathf.Atan(((M * M) - Mathf.Sqrt((M * M * M * M) - G * (G * (DX * DX) + (2.0f * Y0 * (M * M))))) / (G * DX));

        float Theta = -100;

        if (root1 > Theta) Theta = root1;
        if (root2 > Theta) Theta = root2;

        return Theta;
    }

    float solve4T(float M, float DX)
    {
        float G = 9.8f;

        //https://www.wolframalpha.com/input/?i=0+%3D+-g%2F2*x%5E4+%2B+M*x%5E2+-+d

        //float roots[] = {0,0,0,0};

        float root1 = -Mathf.Sqrt(M - Mathf.Sqrt((M * M) - (2.0f * DX * G)) / G);
        float root2 = Mathf.Sqrt(M - Mathf.Sqrt((M * M) - (2.0f * DX * G)) / G);
        float root3 = -Mathf.Sqrt(M + Mathf.Sqrt((M * M) - (2.0f * DX * G)) / G);
        float root4 = Mathf.Sqrt(M + Mathf.Sqrt((M * M) - (2.0f * DX * G)) / G);

        float T = 0;

        if (root1 > T) T = root1;
        if (root2 > T) T = root2;
        if (root1 > T) T = root3;
        if (root4 > T) T = root4;

        return T;
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    void Move()
    {

        int direction2Ball = (int)Mathf.Sign(characterController.hand.position.x - ball.landingPoint) * -1;


        if (aiState == AIState.SEEKBALL)
        {
            animationController.speed = Mathf.Abs(direction2Ball);

            if (chargeingHit /*|| jumping */)
            {
                characterController.Move(direction2Ball, true);
            }
            else
            {
                characterController.Move(direction2Ball, false);
            }
        }
        else
        {
            animationController.speed = 0;
        }
    }

    void Hitting()
    {
        if (Vector2.Distance(ball.transform.position, transform.position) > ball.radious + characterController.handRadious)
        {
            justHitBall = false;
        }

        // hit the ball
        if (!checkingShouldHit)
        {
            checkingShouldHit = true;
            if (ShouldhitBall())
            {
                characterController.setHitMagnitude(GitHitMag());
                animationController.hitBall(GitHitMag() == 2);
                characterController.hitting = true;
                justHitBall = true;
                chargeingHit = false;
                gotServeVector = false;
            }
            checkingShouldHit = false;
        }
    }

    bool ShouldGetAimVector()
    {
        if (aiState == AIState.SERVING)
        {
            if (!gotServeVector)
            {
                gotServeVector = true;
                return true;
            }
        }
        else if (ball.landingPoint != lastBallLandingPoint)
        {
            if (playerNumber == 1 && Mathf.Sign(ball.V.x) * 1 == -1 && ball.landingPoint < 0 ||
                playerNumber == 2 && Mathf.Sign(ball.V.x) * 1 == 1 && ball.landingPoint > 0)
            {
                lastBallLandingPoint = ball.landingPoint;
                return true;
            }
        }
        return false;
    }

    bool ShouldhitBall()
    {
        float aiDistance2Ball = Vector2.Distance(ball.transform.position, transform.position);
        float ballDistnace2Net = Mathf.Abs(GameManager.gameManager.net.transform.position.x - ball.landingPoint);
        float contactDistance = ball.radious + characterController.handRadious;


        if (aiState == AIState.SERVING)
        {
            if (!gotServeVector)
            {
                return false;
            }
            else
            {
                if(ServeWaitTimer == 0)
                {
                    ServeWaitTimer = Time.time;
                    ServeWaitTime = Random.Range(ServeWaitRange.x, ServeWaitRange.y);
                    Debug.Log(ServeWaitTime);
                }
                else if(Time.time - ServeWaitTimer >= ServeWaitTime)
                {
                    ServeWaitTimer = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if (aiDistance2Ball <= contactDistance && // were close enough to the ball
            !justHitBall && // we didnt just hit it
            ballDistnace2Net <= GameManager.gameManager.courtSize && // the ball is in bounds
            angle == angleTo.normalized) // were aiming in the rite direction
        { return true; }

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

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(5);
    }
}
