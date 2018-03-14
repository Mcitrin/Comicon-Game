using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{

    public Animator animator;
    public GameObject HeldBy = null;

    public List<GameObject> Players = new List<GameObject>();
    [Tooltip("If the ball lands here player 1 scores a point [0] is in [1] is out")]
    public List<GameObject> Player1ScoreAreas = new List<GameObject>();
    [Tooltip("If the ball lands here player 2 scores a point [0] is in [1] is out")]
    public List<GameObject> Player2ScoreAreas = new List<GameObject>();
    PlayManager manager;
    Rigidbody rigidbody;
    public bool wait = false; // when the ball lands we wait unitl it has ben reset to hit again
    public ParticleSystem sand;

    float LastHitTime;

    public GameObject collidingPlayer;

    Vector3 inverseAngle;

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
        inverseAngle = -transform.forward;
        Animate();

        if (!GameManager.paused)
        {
            if (rigidbody.isKinematic && !wait) // if the game was paused and not if the ball hit the ground
                rigidbody.isKinematic = false;

            if (HeldBy && !wait && manager.winner == -1) // -1 = is default value
                //transform.position = HeldBy.GetComponent<CharacterController>().hand;

            if (transform.position.y <= .5f)
                transform.position = new Vector3(transform.position.x, .6f, 0);

            if(collidingPlayer)
            {
                checkPlayer(collidingPlayer);
            }
            if(HeldBy)
            {
                collidingPlayer = HeldBy;
            }

            //if (HeldBy == null)
            //this.GetComponent<Rigidbody>().velocity += Vector3.down * .3f;
        }
        else
        {
            if (!rigidbody.isKinematic)
                rigidbody.isKinematic = true;
        }
    }

    void Animate()
    {
        if (HeldBy || rigidbody.isKinematic)
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

    public void HitBall(int power, Vector3 angle)
    {
        int hitPow = 0;
        if (!wait && manager.winner == -1)
        {


            if (HeldBy != null) // if ball is held
            {
                HeldBy = null;
                if (manager.gameState == PlayManager.GameState.waiting)
                    manager.gameState = PlayManager.GameState.PlayingGame;
            }


            if (power == 2)
            {
                if (angle.y <= 0)
                {
                    if (angle.x < 0)
                    {
                        animator.SetBool("Left", true);
                        animator.SetBool("Right", false);
                    }
                    else if (angle.x > 0)
                    {
                        animator.SetBool("Right", true);
                        animator.SetBool("Left", false);
                    }
                }
                hitPow = 13;
                LastHitTime = Time.time;
            }
            else if (power == 1)
            {
                animator.SetBool("Left", false);
                animator.SetBool("Right", false);
                animator.SetBool("Move", true);
                hitPow = 10;
                LastHitTime = Time.time;
            }
            else if (power == 0)
            {
                hitPow = 5;
            }


            
            rigidbody.velocity = angle * hitPow;
            rigidbody.isKinematic = false;
            //Debug.Log(rigidbody.velocity);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        //Debug.Log("on trigger stay");
        //checkPlayer(collision);

    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            collidingPlayer = collision.gameObject;
        }



            if (Player2ScoreAreas.Contains(collision.gameObject) || Player1ScoreAreas.Contains(collision.gameObject))
        {

            wait = true;
            rigidbody.isKinematic = true;// stop ball

            if (Player1ScoreAreas.Contains(collision.gameObject))//ball is in player 1's out or 2's in
            {
                StartCoroutine(ResetBall(1));
            }
            else if (Player2ScoreAreas.Contains(collision.gameObject))//player 2 hit ball in player 1's out
            {
                StartCoroutine(ResetBall(2));
            }

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collidingPlayer = null;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Net" && HeldBy == null)
        {
           /* if (collision.gameObject.name == "netTop")// ball moveing down
            {
                Vector3 point = collision.contacts[0].point;
                Vector3 dir = -collision.contacts[0].normal;

                point -= dir;
                RaycastHit hitInfo;

                if (collision.collider.Raycast(new Ray(point, dir), out hitInfo, 2))
                {
                    Vector3 normal = hitInfo.normal;
                    float angle = Vector3.Angle(-rigidbody.velocity, normal);
                    Quaternion rotate = Quaternion.Euler(0, 0, angle);
                        HitBall(0, rotate * normal, 0, false);

                }
            }
            else
            {*/
            HitBall(0, collision.contacts[0].normal);
            animator.SetBool("Right", false);
            animator.SetBool("Left", false);
            animator.SetBool("Move", true);
            //}
        }
    }

    void checkPlayer(GameObject collision)
    {
        //Debug.Log("checked player");
        if (collision.gameObject.tag == "Player")
        {

            CharacterController Player = collision.gameObject.GetComponentInParent<CharacterController>();
            if (Player.power == 1 || Player.power == 2)
            {
                //Debug.Log(Player.power);
                if (Time.time - LastHitTime >= .25f)
                {
                    HitBall(Player.power, Player.angle);
                }
            }
        }
    }

    IEnumerator ResetBall(int playerNum)
    {
        rigidbody.velocity = Vector3.zero;
        sand.gameObject.SetActive(true);
        manager.WhoScores.SetActive(true);
        manager.WhoScores.GetComponentInChildren<Text>().text = "Player " + playerNum + " Scores!";
        manager.gameState = PlayManager.GameState.waiting;
        yield return new WaitForSeconds(2);
        sand.gameObject.SetActive(false);
        manager.WhoScores.SetActive(false);
        manager.IncrementScore(playerNum);
        if (manager.winner == -1)
        {
            HeldBy = Players[playerNum];
            wait = false;
            //transform.position = HeldBy.GetComponent<CharacterController>().hand;
        }
    }

    // IEnumerator Wait()
    // {
    //     yield return new WaitForSeconds(2);
    //     Debug.Log("wait");
    //     yield return new WaitForSeconds(2);
    // }
}
