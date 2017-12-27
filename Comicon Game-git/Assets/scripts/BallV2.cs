using UnityEngine;
using System.Collections;

public class BallV2 : MonoBehaviour {

    CharacterController player1;
    CharacterController player2;

    const float G = -9.8f;
    Vector3 V;
    float LastHitTime = 0;

    public Animator animator;
    public CharacterController HeldBy = null;
    public ParticleSystem sand;

    public enum BallState
    {
        Held,
        InPlay,
        Reset
    };

    BallState bState = BallState.Held;

    // Use this for initialization
    private void OnEnable()
    {

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
                    CheckForHit(HeldBy);
                    break;

                case BallState.InPlay:

                    if (transform.position.y <= GameManager.ground.position.y)
                    {
                        transform.position = new Vector3(transform.position.x, GameManager.ground.position.y, 0);
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
    
    void CheckForHit(CharacterController Player)
    {
        if (Player.power == 1 || Player.power == 2)
        {
            if (LastHitTime == 0 || Time.time - LastHitTime >= .25f)
            {
                SetVelocity(Player.angle * Player.power * 10);
                LastHitTime = Time.time;
                bState = BallState.InPlay;
            }
        }
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
        Vector3 newV = new Vector3(V.x, V.y + G * Time.deltaTime, 0);
        transform.position = transform.position + (((V + newV) / 2) * Time.deltaTime);
        SetVelocity(newV);
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        bState = BallState.Held;
    }

}
