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

    public GameState gameState = GameState.PlayingGame;

    public ScoarBoard scoreBoard;

    public int P1ScoreCount = 0;
    public int P2ScoreCount = 0;
    public int P1WinCount = 0;
    public int P2WinCount = 0;

    int ClockCount_min = 0;
    int ClockCount_sec = 0;

    int ScoreLimit = 3;
    int WinLimit = 2;
    float TimeLimit = 60;

    float deltaTimeTally;

    public int winner;
    public int roundCount;

    bool reseting;

    // Use this for initialization
    public void Init()
    {
        GameManager.gameManager.ball.pointScored += delegate (int PlayerNumber)
        {
            if (PlayerNumber == 1)
            {
                P1ScoreCount++;
            }
            else if (PlayerNumber == 2)
            {
                P2ScoreCount++;
            }

            if (CheckWinner())
            {
                // somebody won
            }
            else
            {
                // round & match is still going
                StartCoroutine(scoreBoard.PointScored(PlayerNumber, 0, 0, false));
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.paused)
        {
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
    }

    void MainMenu()
    {

    }

    void SetUp()
    {

    }

    void PlayingGame()
    {
        Count();
        scoreBoard.SetNumber("Min", ClockCount_min);
        scoreBoard.SetNumber("Sec", ClockCount_sec);
        scoreBoard.SetNumber("Player1", P1ScoreCount);
        scoreBoard.SetNumber("Player2", P2ScoreCount);
        //scoreBoard.SetTally(P1WinCount, P2WinCount);
    }

    bool CheckWinner()
    {
        bool matchOver;
        if ((P1ScoreCount >= ScoreLimit && P1WinCount + 1 >= WinLimit) || 
            (P2ScoreCount >= ScoreLimit && P2WinCount + 1 >= WinLimit))
        {
            matchOver = true;
        }
        else matchOver = false;

        if (P1ScoreCount >= ScoreLimit)
        {
            P1WinCount++;
            roundCount++;
            StartCoroutine(scoreBoard.PointScored(1, 1, roundCount, matchOver));
            ResetMatch(matchOver);
            return true;
        }
        else if (P2ScoreCount >= ScoreLimit)
        {
            P2WinCount++;
            roundCount++;
            StartCoroutine(scoreBoard.PointScored(2, 2, roundCount, matchOver));
            ResetMatch(matchOver);
            return true;
        }

        return false;

    }

    void ResetMatch(bool done)// done = true for full reset
    {
            ClockCount_min = 0;
            ClockCount_sec = 0;
            P1ScoreCount = 0;
            P2ScoreCount = 0;
            deltaTimeTally = 0;

        if (done)
        {
            P1WinCount = 0;
            P2WinCount = 0;
            roundCount = 0;
        }
    }

    void Count()
    {
        if (ClockCount_min < TimeLimit)
        {
            if (Mathf.FloorToInt(deltaTimeTally) >= 60)
            {
                ClockCount_min++;
                ClockCount_sec = 0;
                deltaTimeTally = 0;
            }
            else
            {
                deltaTimeTally += Time.deltaTime;
                ClockCount_sec = Mathf.FloorToInt(deltaTimeTally);
            }
        }

    }

    public void IncrementScore(int player)
    {
        if (player == 1)
        {

        }
        if (player == 2)
        {
           
        }
    }
}
