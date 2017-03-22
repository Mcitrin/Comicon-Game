using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{

    public Animator animator;
    public GameObject HeldBy = null;
    GameObject LastHit; // last player to hit the ball
    public List<GameObject> Players = new List<GameObject>();
    [Tooltip("If the ball lands here player 1 scores a point [0] is in [1] is out")]
    public List<GameObject> Player1ScoreAreas = new List<GameObject>();
    [Tooltip("If the ball lands here player 2 scores a point [0] is in [1] is out")]
    public List<GameObject> Player2ScoreAreas = new List<GameObject>();
    PlayManager manager;
    Rigidbody rigidbody;
    bool wait = false; // when the ball lands we wait unitl it has ben reset to hit again
    public ParticleSystem sand;
 
    // Use this for initialization
    void Start()
    {
        manager = GameManager.gameManager.GetComponent<PlayManager>();
        rigidbody = GetComponent<Rigidbody>();
        if (Random.value < 0.5f)
            HeldBy = Players[1];
        else
            HeldBy = Players[2];
       
    }

    // Update is called once per frame
    void Update()
    {
        if (HeldBy && !wait && manager.winner == -1) // -1 = is default value
            transform.position = new Vector3(HeldBy.transform.localPosition.x, HeldBy.transform.localPosition.y + .25f,1.2f);
        
        Animate();
    }

    void Animate()
    {
        if(HeldBy)
        {
            animator.SetBool("Right", false);
            animator.SetBool("Left", false);
            animator.SetBool("Move", false);
            animator.SetBool("Held", true);
        }
        else if (!HeldBy && !rigidbody.isKinematic)
        {
            animator.SetBool("Move", true);
            animator.SetBool("Held", false);
        }
    }

    public void HitBall(int power, Vector3 angle, int PlayerNum, bool spike)
    {
        
        if (!wait && manager.winner == -1)
        {
            

            if (HeldBy != null)
            {
                HeldBy = null;
                if (manager.gameState == PlayManager.GameState.waiting)
                manager.gameState = PlayManager.GameState.PlayingGame;
            }

            rigidbody.isKinematic = false;
            if (power == 2)
            {
                if (spike && angle.y <= 0)
                {
                    if (PlayerNum == 2)

                    {
                        animator.SetBool("Left", true);
                        animator.SetBool("Right", false);
                    }
                    else
                    {
                        animator.SetBool("Right", true);
                        animator.SetBool("Left", false);
                    }
                }
                rigidbody.velocity = angle * 13;
            }
            else if (power == 1)
            {
             
             
              animator.SetBool("Left", false);
              animator.SetBool("Right", false);
              animator.SetBool("Move", true);
              rigidbody.velocity = angle * 10;
            }

            LastHit = Players[PlayerNum];
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Player2ScoreAreas.Contains(collision.gameObject) || Player1ScoreAreas.Contains(collision.gameObject))
        {
            rigidbody.isKinematic = true;// stop ball
            //transform.position += new Vector3(0,-.5f,0);

            if (Player1ScoreAreas[0] == collision.gameObject)//player 1 hit ball in player 2's in
            {
                StartCoroutine(ResetBall(1));
            }
            else if (Player1ScoreAreas[1] == collision.gameObject && LastHit != Players[1])//player 2 hit ball in player 1's out
            {
                StartCoroutine(ResetBall(1));
            }
            else if (Player1ScoreAreas[1] == collision.gameObject && LastHit == Players[1])//player 1 hit ball in player 1's out
            {
                StartCoroutine(ResetBall(2));
            }
            if (Player2ScoreAreas[0] == collision.gameObject)//player 2 hit ball in player 1's in
            {
                StartCoroutine(ResetBall(2));
            }
            else if (Player2ScoreAreas[1] == collision.gameObject && LastHit != Players[2])//player 1 hit ball in player 2's out
            {
                StartCoroutine(ResetBall(2));
            }
            else if (Player2ScoreAreas[1] == collision.gameObject && LastHit == Players[2])//player 2 hit ball in player 2's in
            {
                StartCoroutine(ResetBall(1));
            }
        }
    }

    IEnumerator ResetBall(int playerNum)
    {
        animator.SetBool("Right", false);
        animator.SetBool("Left", false);
        animator.SetBool("Move", false);
        animator.SetBool("Held", true);
        sand.gameObject.SetActive(true);
        wait= true;
        manager.gameState = PlayManager.GameState.waiting;
        yield return new WaitForSeconds(2);
        sand.gameObject.SetActive(false);
        manager.IncrementScore(playerNum);
        HeldBy = Players[playerNum];
        wait = false;

    }

   // IEnumerator Wait()
   // {
   //     yield return new WaitForSeconds(2);
   //     Debug.Log("wait");
   //     yield return new WaitForSeconds(2);
   // }
}
