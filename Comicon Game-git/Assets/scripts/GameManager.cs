using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private static GameManager _gameManager;
    public static GameManager gameManager
    {
        get
        {
            return _gameManager;
        }
    }

    public InputMan inputMan;
    public static bool paused;
    public static int numPlayers = 2;
    public List<CharacterController> Players = new List<CharacterController>();
    public BallV2 ball;

    public Transform left;
    public Transform right;
    public Transform net;
    public Transform ground;


    public int hardHit = 18;
    public int normHit = 15;

    public float courtSize;


    public float hitMag = 15;

    public GameObject pauseCanvas;

    public bool player1IsAI;
    public bool player2IsAI;


    // Use this for initialization
    private void Awake()
    {
        _gameManager = this;
        inputMan = GetComponent<InputMan>();

        DontDestroyOnLoad(gameObject);
        Random.seed = (int)System.DateTime.Now.Ticks;

        ball.Init();
        Players[0].Init();
        Players[1].Init();

        courtSize = net.position.x - left.position.x;

    }

    // called after Awake
    void Start () {
       
    }

   public void ChangePlayerController(int playerNum)
    {
        if(playerNum == 1)
        {
            player1IsAI = !player1IsAI;
            Players[0].GetComponent<AIController>().enabled = !Players[0].GetComponent<AIController>().enabled;
            Players[0].GetComponent<PlayerController>().enabled = !Players[0].GetComponent<PlayerController>().enabled;
        }

        if (playerNum == 2)
        {
            player2IsAI = !player2IsAI;
            Players[1].GetComponent<AIController>().enabled = !Players[1].GetComponent<AIController>().enabled;
            Players[1].GetComponent<PlayerController>().enabled = !Players[1].GetComponent<PlayerController>().enabled;
        }
    }

	// Update is called once per frame
	void Update () {
     pauseCanvas.SetActive(paused);

        
        if (inputMan.Pause())
        {
            paused = !paused;
        }

    }
}
