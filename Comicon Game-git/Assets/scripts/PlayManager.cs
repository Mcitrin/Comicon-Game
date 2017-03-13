using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayManager : MonoBehaviour {

    public enum GameState
    {
        MainMenu,
        SetUp,
        waiting,
        PlayingGame
    };

    public InputMan inputMan;
    public GameState gameState = GameState.MainMenu;

    public GameObject TimeScoreWarning;
    public GameObject ControllerWarning;
    public Text ScoreLabel;
    public Text TimeLabel;
    public int ScoreIndex;
    public int TimeIndex;
    GameObject P1ScoreDisplay;
    GameObject P2ScoreDisplay;
    GameObject ClockDisplay;
    int P1ScoreCount;
    int P2ScoreCount;
    int ClockCount;

    int ScoreLimit;
    int TimeLimit;

    int interval;
    float nextTime;
    int seconds;


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
                PlayingGame();

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
        ClockDisplay = GameObject.Find("Clock");
        P1ScoreDisplay = GameObject.Find("P1Score");
        P2ScoreDisplay = GameObject.Find("P2Score");

        interval = 1;
        nextTime = 0;
        seconds = 0;

        if (ClockDisplay != null && P1ScoreDisplay != null && P2ScoreDisplay != null)
        {
            ClockCount = TimeLimit;
            ClockDisplay.GetComponent<Text>().text = (ClockCount / 60) + ":" + "00";
            gameState = GameState.waiting;
        }
    }

    void PlayingGame()
    {
        if(ClockCount !=0)
        Count();

        if (ClockCount <= 0 && TimeLimit != 0)
        CalculateWinner();
    }

  

    void CalculateWinner()
    {
        if(P1ScoreCount > P2ScoreCount)
        {
            Debug.Log("Player1 wins!");
        }
        else
        {
            Debug.Log("Player2 wins!");
        }
        Application.LoadLevel("Menu");
        Destroy(gameObject);
    }

    void Count()
    {
        if (Time.time >= nextTime)
        {
            ClockCount--;
            seconds--;
            if (seconds < 0) { seconds = 59; }
            if (seconds >= 10)
                ClockDisplay.GetComponent<Text>().text = (ClockCount / 60) + ":" + seconds;
            else
                ClockDisplay.GetComponent<Text>().text = (ClockCount / 60) + ":0" + seconds;

            nextTime = (int)Time.time + interval;
        }
    }

    public void IncrementScore(int player)
    {
        if(player == 1)
        {
            P1ScoreCount++;
            P1ScoreDisplay.GetComponent<Text>().text = "" + P1ScoreCount;
        }
        if(player == 2)
        {
            P2ScoreCount++;
            P2ScoreDisplay.GetComponent<Text>().text = "" + P2ScoreCount;
        }

        if (ScoreLimit != 0)
        {
            if (P1ScoreCount == ScoreLimit || P2ScoreCount == ScoreLimit)
            {
                CalculateWinner();
            }
        }
    }
}
