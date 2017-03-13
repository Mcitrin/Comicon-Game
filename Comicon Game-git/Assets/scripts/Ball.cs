using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{

    public GameObject HeldBy = null;
    GameObject LastHit; // last player to hit the ball
    public List<GameObject> Players = new List<GameObject>();
    [Tooltip("If the ball lands here player 1 scores a point [0] is in [1] is out")]
    public List<GameObject> Player1ScoreAreas = new List<GameObject>();
    [Tooltip("If the ball lands here player 2 scores a point [0] is in [1] is out")]
    public List<GameObject> Player2ScoreAreas = new List<GameObject>();
    PlayManager manager;
    Rigidbody rigidbody;


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
        if (HeldBy)
            transform.position = new Vector3(HeldBy.transform.localPosition.x, HeldBy.transform.localPosition.y + .5f, -.5f);

    }

    public void HitBall(int power, Vector3 angle, int PlayerNum)
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
            rigidbody.velocity = angle * 13.0f;//12
        }
        else if (power == 1)
        {
            rigidbody.velocity = angle * 10.0f;//9
        }

        LastHit = Players[PlayerNum];
    }


    void OnCollisionEnter(Collision collision)
    {
        if (Player2ScoreAreas.Contains(collision.gameObject) || Player1ScoreAreas.Contains(collision.gameObject))
        {
            rigidbody.isKinematic = true;// stop ball

            if (Player1ScoreAreas[0] == collision.gameObject)//player 1 hit ball in player 2's in
            {
                ResetBall(1);
            }
            else if (Player1ScoreAreas[1] == collision.gameObject && LastHit != Players[1])//player 2 hit ball in player 1's out
            {
                ResetBall(1);
            }
            else if (Player1ScoreAreas[1] == collision.gameObject && LastHit == Players[1])//player 1 hit ball in player 1's out
            {
                ResetBall(2);
            }
            if (Player2ScoreAreas[0] == collision.gameObject)//player 2 hit ball in player 1's in
            {
                ResetBall(2);
            }
           else if (Player2ScoreAreas[1] == collision.gameObject && LastHit != Players[2])//player 1 hit ball in player 2's out
            {
                ResetBall(2);
            }
            else if (Player2ScoreAreas[1] == collision.gameObject && LastHit == Players[2])//player 2 hit ball in player 2's in
            {
                ResetBall(1);
            }
        }
    }

    void ResetBall(int playerNum)
    {
        manager.IncrementScore(playerNum);
        manager.gameState = PlayManager.GameState.waiting;
        HeldBy = Players[playerNum];
    }
}
