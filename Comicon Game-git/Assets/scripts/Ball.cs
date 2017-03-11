using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    public GameObject HeldBy = null;
    public GameObject player1;
    public GameObject player2;
    PlayManager manager;

	// Use this for initialization
	void Start () {
        manager = GameManager.gameManager.GetComponent<PlayManager>();
        if (Random.value < 0.5f)
            HeldBy = player1;
        else
            HeldBy = player2;
	}
	
	// Update is called once per frame
	void Update () {
        if (HeldBy)
            transform.position = new Vector3(HeldBy.transform.localPosition.x, HeldBy.transform.localPosition.y,-.5f);
        
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
            GetComponent<Rigidbody>().velocity = angle * 13.0f;//12
        }
        else if (power == 1)
        {
            GetComponent<Rigidbody>().velocity = angle * 10.0f;//9
        }

    }
}
