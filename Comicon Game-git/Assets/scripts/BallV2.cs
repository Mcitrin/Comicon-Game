﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallV2 : MonoBehaviour {

    public CharacterController lastPlayerHit = null;
    float G = -9.8f;
    public Vector3 V = Vector3.zero;

    public Animator animator;
    public CharacterController HeldBy;
    public ParticleSystem sand;

    public char CourtSide;

    bool reseting = false;

    float radious;
    float yBounds;

    Vector2 net;
    bool recentlyHitNet;

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
                    Vector3 V1 = new Vector3(player.getHitVector().x, player.getHitVector().y) * (player.getHitVector().z * 10);
                    SetVelocity(V1);
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
                        SetVelocity(new Vector3(-V.x * .5f, V.y * .5f, 0));
                        recentlyHitNet = true; // we just hit the net
                        //lastPlayerHit = null; // allow the player to hit the ball again
                    }
                }
               else if (CourtSide == 'R') // right side of court
                {
                    if (transform.position.x - radious <= net.x)
                    {
                        // net animate left
                        SetVelocity(new Vector3(-V.x * .5f, V.y * .5f, 0));
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
                    SetVelocity(new Vector3(Mathf.Sign(V.x) * 2, -V.y * .5f, 0));
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
    }

    void SetVelocity(Vector3 V1)
    {
        V = V1;
        if (bState == BallState.Held) bState = BallState.InPlay;
    }

    void Integrate()
    {
        Vector3 newV = Vector3.zero;

        if (V.y > 0)
        {
             newV = new Vector3(V.x, V.y + G * Time.deltaTime, 0);
        }
        else
        {
             newV = new Vector3(V.x, V.y + (G * 2.5f) * Time.deltaTime, 0);
        }

        transform.position = transform.position + (((V + newV) / 2) * (Time.deltaTime * .85f));
        SetVelocity(newV);
    }

    IEnumerator Reset()
    {
        reseting = true;
        yield return new WaitForSeconds(3);
        bState = BallState.Held;
        reseting = false;
    }

}
