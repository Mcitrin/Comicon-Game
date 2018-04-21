using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallV2 : MonoBehaviour {

    public CharacterController lastPlayerHit = null;
    float G = -9.8f;
    public Vector3 V = Vector3.zero;
    Vector3 V1 = Vector3.zero;

    public Animator animator;
    public Animator netting;
    public CharacterController HeldBy;
    public ParticleSystem sand;

    public SlowTime slowTime;

    public char CourtSide;

    bool reseting = false;

    public float landingPoint = 0;
    public float radious;
    float yBounds;

    Vector2 net;
    bool recentlyHitNet;

    float tmpTime = 0;

    // Evants
    public delegate void PointScored(int playerNumber);
    public event PointScored pointScored;

    public enum BallState
    {
        Held,
        InPlay,
        Reset
    };

    public BallState bState = BallState.Held;

    // Use this for initialization
     public void Init()
    {
        HeldBy = GameManager.gameManager.Players[Random.Range(0,2)];
        radious = GetComponent<CircleCollider2D>().radius;
        net = GameManager.gameManager.net.position;
    }

    // Update is called once per frame
    void Update ()
    {
        PlayerColission();
        CheckCourtSide();

        if (!GameManager.paused)
        {
            switch (bState)
            {
                case BallState.Held:
                    transform.position = HeldBy.GetComponent<CharacterController>().hand.position;
                    break;

                case BallState.InPlay:

                    NetColission();

                    if (transform.position.y <= GameManager.gameManager.ground.position.y)
                    {
                        transform.position = new Vector3(transform.position.x, GameManager.gameManager.ground.position.y, 0);
                        V = Vector3.zero;
                        bState = BallState.Reset;
                    }
                    else
                    {
                        Integrate();
                    }
                    break;

                case BallState.Reset:
                    if (!reseting)
                    {
                        reseting = true;
                        StartCoroutine(Reset());
                    }
                    break;
                default:
                    break;
            }
        }


    }

    void PlayerColission()
    {
        foreach (var player in GameManager.gameManager.Players)
        {
            // if the players hand in range of teh ball
            if (Vector2.Distance(transform.position, player.hand.position) <= player.handRadious)
            {
                // is the player trying to hit the ball and have they already done so
                if (player.hitting && lastPlayerHit != player)
                {
                    if (player.getHitVector().z == 1)
                    {
                         V1 = new Vector3(player.getHitVector().x, player.getHitVector().y) * (GameManager.gameManager.normHit);
                    }
                    if (player.getHitVector().z == 2)
                    {
                         V1 = new Vector3(player.getHitVector().x, player.getHitVector().y) * (GameManager.gameManager.hardHit);
                    }
                    SetVelocity(V1, true);
                    lastPlayerHit = player; 
                    recentlyHitNet = false; // allow the ball to hit the net again
                    CheckAnimationState();
                }
            }
            else // if were not in range
            {   // and if were the last to hit
                if (lastPlayerHit == player)
                {
                    lastPlayerHit = null;
                }
            }
        }
    }

    void NetColission()
    {
        if (!recentlyHitNet)
        {
            if (transform.position.y < net.y) // below net
            {
                if (CourtSide == 'L') // left side of court
                {
                    if (transform.position.x + radious >= net.x)
                    {
                        netting.SetTrigger("right");
                        SetVelocity(new Vector3(-V.x * .5f, V.y * .5f, 0), true);
                        recentlyHitNet = true; // we just hit the net
                        SetAimation("Stop");
                    }
                }
               else if (CourtSide == 'R') // right side of court
                {
                    if (transform.position.x - radious <= net.x)
                    {
                        netting.SetTrigger("left");
                        SetVelocity(new Vector3(-V.x * .5f, V.y * .5f, 0), true);
                        recentlyHitNet = true;
                        SetAimation("Stop");
                    }
                }
            }
            else // above net
            {
                if (transform.position.y - radious <= net.y && V.y < 0 
                    && transform.position.x <= net.x + radious
                    && transform.position.x >= net.x - radious)
                {
                    SetVelocity(new Vector3(Mathf.Sign(V.x) * 2, -V.y * .5f, 0), true);
                    recentlyHitNet = true; // we just hit the net
                    SetAimation("Stop");
                }

            }
        }
        else
        {
            if (Vector2.Distance(transform.position, net) <= radious)
            {
                recentlyHitNet = false;
            }
        }
    }

    void Animate()
    {

    }

    void CheckCourtSide()
    {
        if(transform.position.x < net.x)
        {
            CourtSide = 'L';
        }
        else
        {
            CourtSide = 'R';
        }

        if(transform.position.x > GameManager.gameManager.right.position.x 
            || transform.position.x < GameManager.gameManager.left.position.x)
        {
            CourtSide = 'O';
        }
    }

    void SetVelocity(Vector3 V1, bool getLandingPoint)
    {
        V = V1;
        if (bState == BallState.Held)
        {
            bState = BallState.InPlay;
            HeldBy = null;
        }

        if(getLandingPoint) // if this is true the ball has either the net or a player
        {
            solve4DX();
        }
    }

    float solve4T()
    {
        float T = 0;
        float Y0 = transform.position.y;
        float Y1 = GameManager.gameManager.ground.transform.position.y ;

        float plus = (-V.y + Mathf.Sqrt((V.y * V.y) - (2.0f * G) * Y0)) / G;
        float minus = (-V.y - Mathf.Sqrt((V.y * V.y) - (2.0f * G) * Y0)) / G;

        if (plus > 0) { T = plus; }
        else if (minus > 0) { T = minus; }
        //Debug.Log(T);
        return T;
    }

    void solve4DX()
    {
        float T = solve4T();
        float DX = GameManager.gameManager.ball.transform.position.x + (V.x * T);
        landingPoint = DX;
    }

    void Integrate()
    {
        Vector3 newV = Vector3.zero;
        newV = new Vector3(V.x, V.y + G * Time.deltaTime, 0);

        transform.position = transform.position + (((V + newV) * .5f) * (Time.deltaTime));
        SetVelocity(newV, false);
    }

    void CheckAnimationState()
    {
        if (V.magnitude >= GameManager.gameManager.hardHit
          && V1.y < 0 && lastPlayerHit != null)
        {
            if (V.x > 0) SetAimation("Right");
            else if (V.x < 0) SetAimation("Left");
            V1 = V1.normalized * 22;
        }
        else
        {
            SetAimation("Stop");
        }
    }

    void SetAimation(string state)
    {
        animator.SetBool("Left", false);
        animator.SetBool("Right", false);
        if(state != "Stop")
        animator.SetBool(state, true);
    }

    IEnumerator Reset()
    {
        SetAimation("Stop");
        int scorer = WhoScored();
        if(scorer != 0)
        {
            if (pointScored != null)
                pointScored(scorer);

            HeldBy = GameManager.gameManager.Players[scorer - 1];
        }

        yield return new WaitForSeconds(4);
        bState = BallState.Held;
        landingPoint = 0;

     

        reseting = false;
    }

    int WhoScored()
    {
        // p2 scores
        if(CourtSide == 'L')
        {
            return 2;
        }
        // p1 scores
        else if(CourtSide == 'R')
        {
            return 1;
        }
        // balls out of bounds
        else if (CourtSide == 'O')
        {
            // out of bounds p2 scores
            if (transform.position.x > GameManager.gameManager.right.position.x)
            {
                return 2;
            }
            // out of bounds p1 scores
            if (transform.position.x < GameManager.gameManager.left.position.x)
            {
                return 1;
            }
        }
        return 0;
    }

}
