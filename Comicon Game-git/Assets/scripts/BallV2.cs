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
        HeldBy = GameManager.gameManager.Players[0];
        xBounds = GetComponent<BoxCollider2D>().bounds.extents.x;
        yBounds = GetComponent<BoxCollider2D>().bounds.extents.y;

        GameManager.gameManager.Players[0].hitBallAttempt += delegate (Vector2 angle, float pow, Vector2 handBounds, Vector2 handPosition)
        {
            if(InHand(handBounds, handPosition))
            {
                SetVelocity(angle * (pow*10));
                if(bState == BallState.Held)
                {
                    bState = BallState.InPlay;
                }
            }
        };

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
    
    bool InHand(Vector2 handBounds, Vector2 handPosition)
    {
        if (transform.position.x + xBounds <= handPosition.x + handBounds.x &&
           transform.position.x - xBounds >= handPosition.x - handBounds.x &&
           transform.position.y + yBounds <= handPosition.y + handBounds.y &&
           transform.position.y - yBounds >= handPosition.y - handBounds.y)
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

        transform.position = transform.position + (((V + newV) / 2) * (Time.deltaTime * .85f));
        SetVelocity(newV);
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        bState = BallState.Held;
    }

}
