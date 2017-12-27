using UnityEngine;
using System.Collections;

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
    public CharacterController player1;
    public CharacterController player2;
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
    }

    // called after Awake
    void Start () {
        DontDestroyOnLoad(gameObject);
        Random.seed = (int)System.DateTime.Now.Ticks;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
