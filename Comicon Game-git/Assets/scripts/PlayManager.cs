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

    int P1ScoreCount = 0;
    int P2ScoreCount = 0;
    int ClockCount_min = 0;
    int ClockCount_sec = 0;

    int ScoreLimit = 25;
    float TimeLimit = 60;

    float deltaTimeTally;

    public int winner;


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
            StartCoroutine(scoreBoard.PointScored(PlayerNumber));
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
    }

    void CalculateWinner()
    {


    }

    IEnumerator WaitBeforReset(int winner)
    {
        yield return new WaitForSeconds(10);
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
