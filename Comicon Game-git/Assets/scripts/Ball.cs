using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{

    public GameObject HeldBy = null;
    public GameObject player1;
    public GameObject player2;
    [Tooltip("If the ball lands here player 1 scores a point")]
    public List<GameObject> Player1ScoreAreas = new List<GameObject>();
    [Tooltip("If the ball lands here player 2 scores a point")]
    public List<GameObject> Player2ScoreAreas = new List<GameObject>();
    PlayManager manager;
    Rigidbody rigidbody;


    // Use this for initialization
    void Start()
    {
        manager = GameManager.gameManager.GetComponent<PlayManager>();
        rigidbody = GetComponent<Rigidbody>();
        if (Random.value < 0.5f)
            HeldBy = player1;
        else
            HeldBy = player2;
    }

    // Update is called once per frame
    void Update()
    {
        if (HeldBy)
            transform.position = new Vector3(HeldBy.transform.localPosition.x, HeldBy.transform.localPosition.y, -.5f);

    }

    public void HitBall(int power, Vector3 angle)
    {
        if (HeldBy != null)
        {
            HeldBy = null;
            manager.gameState = PlayManager.GameState.PlayingGame;
        }

        if (power == 2)
        {
            rigidbody.velocity = angle * 13.0f;//12
        }
        else if (power == 1)
        {
            rigidbody.velocity = angle * 10.0f;//9
        }

    }


    void OnCollisionEnter(Collision collision)
    {
      if (Player2ScoreAreas.Contains(collision.gameObject) || Player1ScoreAreas.Contains(collision.gameObject))
        {
          rigidbody.isKinematic = true;// stop ball

            if (Player1ScoreAreas.Contains(collision.gameObject))
                Debug.Log("Player1 scores");
            else if (Player2ScoreAreas.Contains(collision.gameObject))
                Debug.Log("Player2 scores");
        }
    }
}
