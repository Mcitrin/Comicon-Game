using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallV2 : MonoBehaviour {

    List<CharacterController> Players = new List<CharacterController>();

    float G = -9.8f;
    Vector3 V = Vector3.zero;
    public float LastHitTime = 0;

    public Animator animator;
    public CharacterController HeldBy;
    public ParticleSystem sand;

    bool checkingForHit = false;

    float xBounds;
    float yBounds;

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
        Players = GameManager.gameManager.Players;
        HeldBy = Players[0];
        xBounds = GetComponent<BoxCollider2D>().bounds.extents.x;
        yBounds = GetComponent<BoxCollider2D>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!GameManager.paused)
        {
            switch (bState)
            {
                case BallState.Held:

                    transform.position = HeldBy.GetComponent<CharacterController>().hand.transform.position;
                    if(!checkingForHit)
                    CheckForHit(HeldBy);
                    break;

                case BallState.InPlay:

                    if (transform.position.y <= GameManager.gameManager.ground.position.y)
                    {
                        transform.position = new Vector3(transform.position.x, GameManager.gameManager.ground.position.y, 0);
                        V = Vector3.zero;
                        bState = BallState.Reset;
                    }
                    else
                    {
                        for (int i = 0; i < Players.Capacity; i++)
                        {
                            if (!checkingForHit)
                                CheckForHit(Players[i]);
                        }
                        
                        Integrate();
                    }
                    break;

                case BallState.Reset:

                    StartCoroutine(Reset());
                    break;
                default:
                    break;
            }
        }


    }
    
    void CheckForHit(CharacterController Player)
    {
        checkingForHit = true;

        if (Player.power > 0 && InHand(Player))
        {
            if (LastHitTime == 0 || Time.time - LastHitTime >= .25f)
            {
                SetVelocity(Player.angle * Player.power * 10);
                LastHitTime = Time.time;

                if (HeldBy != null)
                {
                    bState = BallState.InPlay;
                    HeldBy = null;
                }
                Debug.Log("HittingBall");
            }
        }
        checkingForHit = false;
    }

    bool InHand(CharacterController Player)
    {
        if (transform.position.x + xBounds <= Player.hand.transform.position.x + Player.handXBounds &&
           transform.position.x - xBounds >= Player.hand.transform.position.x - Player.handXBounds &&
           transform.position.y + yBounds <= Player.hand.transform.position.y + Player.handYBounds &&
           transform.position.y - yBounds >= Player.hand.transform.position.y - Player.handYBounds)
            return true;
        else
            return false;
    }

    void Animate()
    {

    }

    void SetVelocity(Vector3 V1)
    {
        V = V1;
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

        transform.position = transform.position + (((V + newV) / 2) * Time.deltaTime);
        SetVelocity(newV);
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        LastHitTime = 0;
        bState = BallState.Held;
        HeldBy = Players[0];
    }

}
