using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayManager : MonoBehaviour
{

    public enum GameState
    {
        MainMenu,
        SetUp,
        waiting,
        PlayingGame
    };

    public InputMan inputMan;
    public PlayerInfo playerInfo;
    public GameState gameState = GameState.MainMenu;

    public GameObject TimeScoreWarning;
    public GameObject ControllerWarning;
    public Text P1WinsLabel;
    public Text P2WinsLabel;
    public Text ScoreLabel;
    public Text TimeLabel;
    public int ScoreIndex;
    public int TimeIndex;
    GameObject P1ScoreDisplay;
    GameObject P2ScoreDisplay;
    GameObject ClockDisplay;
    GameObject Crown;
    GameObject PauseMenu;
   public GameObject WhoScores;
    int P1ScoreCount;
    int P2ScoreCount;
    int ClockCount;

    int ScoreLimit;
    int TimeLimit;

    int interval;
    float nextTime;
    int seconds;

    bool crowned = false;
    public int winner;
    public bool paused;


    // Use this for initialization
    void Awake()
    {
        inputMan = GameManager.gameManager.GetComponent<InputMan>();
        playerInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            if (gameState != GameState.MainMenu && gameState != GameState.SetUp && PauseMenu.activeInHierarchy == true) { PauseMenu.SetActive(false);} // if we have handle on pause panel and player is not pausing disable pannel

            switch (gameState)
            {
                case GameState.MainMenu:
                    MainMenu();
                    break;
                case GameState.SetUp:
                    SetUp();
                    break;
                case GameState.waiting:
                    if (crowned)
                    {
                        Crown.transform.position = GameObject.Find("Player" + winner).transform.position + Vector3.up * 1.75f;
                        Crown.transform.Rotate(Vector3.up, .3f);
                    }

                    break;
                case GameState.PlayingGame:
                    PlayingGame();

                    break;
            }
        }
        else
        {
            if (PauseMenu.activeInHierarchy == false) { PauseMenu.SetActive(true); } // else in enalbe pannel if player is pausing
        }


    }

    void MainMenu()
    {
        P1WinsLabel.text = "Wins: " + playerInfo.P1Wins;
        P2WinsLabel.text = "Wins: " + playerInfo.P2Wins;

        if (ScoreIndex == 0 && TimeIndex == 0)
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
        
        
        setGameSettings(ScoreIndex, TimeIndex);
    }

    void SetUp()
    {
        ClockDisplay = GameObject.Find("Clock");
        P1ScoreDisplay = GameObject.Find("P1Score");
        P2ScoreDisplay = GameObject.Find("P2Score");
        Crown = GameObject.Find("Crown");
        PauseMenu = GameObject.Find("PauseMenu");
        WhoScores = GameObject.Find("WhoScores");

        interval = 1;
        nextTime = 0;
        seconds = 0;

        if (ClockDisplay != null &&
            P1ScoreDisplay != null &&
            P2ScoreDisplay != null &&
            Crown != null &&
            PauseMenu != null &&
            WhoScores != null
            )
        {
            ClockCount = TimeLimit;
            ClockDisplay.GetComponent<Text>().text = (ClockCount / 60) + ":" + "00";
            PauseMenu.SetActive(false);
            WhoScores.SetActive(false);
            gameState = GameState.waiting;
        }
    }

    void PlayingGame()
    {
        if (ClockCount != 0)
            Count();

        if (ClockCount <= 0 && TimeLimit != 0)
            CalculateWinner();
    }

    void CalculateWinner()
    {

        WhoScores.SetActive(true);
        if (P1ScoreCount > P2ScoreCount)
        {
            winner = 1;
            playerInfo.P1Wins++;
            WhoScores.GetComponentInChildren<Text>().text = "  Player 1 Wins!";
        }
        else if (P1ScoreCount < P2ScoreCount)
        {
            winner = 2;
            playerInfo.P2Wins++;
            WhoScores.GetComponentInChildren<Text>().text = "  Player 2 Wins!";
        }
        else
        {
            winner = 0; // Draw
            WhoScores.GetComponentInChildren<Text>().text = "Draw";
        }
        StartCoroutine(WaitBeforReset(winner));

    }

    IEnumerator WaitBeforReset(int winner)
    {
        if (winner != 0)
        {
            crowned = true;
        }

        yield return new WaitForSeconds(10);
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
        if (player == 1)
        {
            P1ScoreCount++;
            P1ScoreDisplay.GetComponent<Text>().text = "" + P1ScoreCount;
        }
        if (player == 2)
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

    public void setGameSettings(int ScoreI,int TimeI)
    {
        switch (ScoreI)
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
        switch (TimeI)
        {
            case 0:
                TimeLabel.text = "0:00";
                TimeLimit = 0;
                break;
            case 1:
                TimeLabel.text = "5:00";
                TimeLimit = 5 * 60;
                break;
            case 2:
                TimeLabel.text = "10:00";
                TimeLimit = 10 * 60;
                break;
            case 3:
                TimeLabel.text = "15:00";
                TimeLimit = 15 * 60;
                break;
            case 4:
                TimeLabel.text = "20:00";
                TimeLimit = 20 * 60;
                break;
        }

    }
}
