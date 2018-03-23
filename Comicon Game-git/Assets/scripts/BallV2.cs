﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallV2 : MonoBehaviour {

    public CharacterController lastPlayerHit = null;
    float G = -9.8f;
    public Vector3 V = Vector3.zero;
    Vector3 V1 = Vector3.zero;

    public Animator animator;
    public CharacterController HeldBy;
    public ParticleSystem sand;

    public char CourtSide;

    bool reseting = false;

    public float landingPoint = 0;
    public float radious;
    float yBounds;

    Vector2 net;
    bool recentlyHitNet;

    float tmpTime = 0;

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
        HeldBy = GameManager.gameManager.Players[0];
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
                    if (!reseting) StartCoroutine(Reset());
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
                         V1 = new Vector3(player.getHitVector().x, player.getHitVector().y) * (player.getHitVector().z * 15);
                    }
                    if (player.getHitVector().z == 2)
                    {
                         V1 = new Vector3(player.getHitVector().x, player.getHitVector().y) * (player.getHitVector().z * 9);
                    }
                    SetVelocity(V1, true);
                    lastPlayerHit = player; 
                    recentlyHitNet = false; // allow the ball to hit the net again
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
                        // net animate right
                        SetVelocity(new Vector3(-V.x * .5f, V.y * .5f, 0), true);
                        recentlyHitNet = true; // we just hit the net
                        //lastPlayerHit = null; // allow the player to hit the ball again
                    }
                }
               else if (CourtSide == 'R') // right side of court
                {
                    if (transform.position.x - radious <= net.x)
                    {
                        // net animate left
                        SetVelocity(new Vector3(-V.x * .5f, V.y * .5f, 0), true);
                        recentlyHitNet = true; 
                        //lastPlayerHit = null; 
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
        if (bState == BallState.Held) bState = BallState.InPlay;

        if(getLandingPoint)
        {
           solve4DX();
            tmpTime = Time.time;
        }
    }

    float solve4T()
    {
        float T = 0;
        float Y0 = transform.position.y;
        float Y1 = GameManager.gameManager.ground.transform.position.y;

        float plus = (-V.y + Mathf.Sqrt((V.y * V.y) - (2 * G) * Y0)) / G;
        float minus = (-V.y - Mathf.Sqrt((V.y * V.y) - (2 * G) * Y0)) / G;

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

        transform.position = transform.position + (((V + newV) / 2) * (Time.deltaTime));
        SetVelocity(newV, false);
    }

    IEnumerator Reset()
    {
        reseting = true;
        //Debug.Log(Time.time - tmpTime);
        yield return new WaitForSeconds(3);
        bState = BallState.Held;
        landingPoint = 0;
        reseting = false;
    }

}
