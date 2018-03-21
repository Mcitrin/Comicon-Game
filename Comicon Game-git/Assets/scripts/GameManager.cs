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
    public static int numPlayers = 1;
    public List<CharacterController> Players = new List<CharacterController>();
    public BallV2 ball;

    public Transform left;
    public Transform right;
    public Transform net;
    public Transform ground;



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

    }

    // called after Awake
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
