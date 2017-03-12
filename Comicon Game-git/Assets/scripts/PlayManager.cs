using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayManager : MonoBehaviour {

    public enum GameState
    {
        MainMenu,
        SetUp,
        waiting,
        PlayingGame,
        Restarting
    };

    public InputMan inputMan;
    public GameState gameState = GameState.MainMenu;

    public GameObject TimeScoreWarning;
    public GameObject ControllerWarning;
    public Text ScoreLabel;
    public Text TimeLabel;
    public int ScoreIndex;
    public int TimeIndex;
    GameObject P1Score;
    GameObject P2Score;
    GameObject Clock;
    int P1ScoreCount;
    int P2ScoreCount;
    int ClockCount;

    int ScoreLimit;
    int TimeLimit;
    

    // Use this for initialization
    void Awake () {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
    }

    // Update is called once per frame
    void Update() {
        switch (gameState)
        {
            case GameState.MainMenu:
                MainMenu();
                break;
            case GameState.SetUp:
                SetUp();
                break;
            case GameState.waiting:

                break;
            case GameState.PlayingGame:

                break;
            case GameState.Restarting:

                break;
        }

    }

    void MainMenu()
    {
        if(ScoreIndex == 0 && TimeIndex == 0)
        {
            TimeScoreWarning.SetActive(true);
        }
        else
        {
            TimeScoreWarning.SetActive(false);
        }

        if (inputMan.NoJoysticks)
        {
            ControllerWarning.SetActive(true);
        }
        else
        {
            ControllerWarning.SetActive(false);
        }


        switch (ScoreIndex)
        {
            case 0:
                ScoreLabel.text = "0";
                ScoreLimit = 0;
                break;
            case 1:
                ScoreLabel.text = "10";
                ScoreLimit = 10;
                break;
            case 2:
                ScoreLabel.text = "25";
                ScoreLimit = 25;
                break;
            case 3:
                ScoreLabel.text = "50";
                ScoreLimit = 50;
                break;
            case 4:
                ScoreLabel.text = "100";
                ScoreLimit = 100;
                break;
        }
        switch (TimeIndex)
        {
            case 0:
                TimeLabel.text = "0:00";
                TimeLimit = 0;
                break;
            case 1:
                TimeLabel.text = "5:00";
                TimeLimit = 5*60;
                break;
            case 2:
                TimeLabel.text = "10:00";
                TimeLimit = 10*60;
                break;
            case 3:
                TimeLabel.text = "15:00";
                TimeLimit = 15*60;
                break;
            case 4:
                TimeLabel.text = "20:00";
                TimeLimit = 20*60;
                break;
        }
    }

    void SetUp()
    {
        Clock = GameObject.Find("Clock");
        P1Score = GameObject.Find("P1Score");
        P2Score = GameObject.Find("P2Score");

        if(Clock !=null && P1Score != null && P2Score !=null)
        {
            Clock. GetComponent<Text>().text = (TimeLimit / 60) + ":00";
            gameState = GameState.waiting;
        }
    }

    public void IncrementScore(int player)
    {
        //if(player == 1)
        //if(player == 2)
    }
}
