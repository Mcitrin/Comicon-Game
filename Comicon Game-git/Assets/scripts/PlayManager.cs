using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayManager : MonoBehaviour {

    public enum GameState
    {
        MainMenu,
        waiting,
        PlayingGame,
        Restarting
    };

    public GameState gameState = GameState.MainMenu;

    public GameObject Warning;
    public Text ScoreLabel;
    public Text TimeLabel;
    public int ScoreIndex;
    public int TimeIndex;
    int ScoreLimit;
    int TimeLimit;
    

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update() {
        switch (gameState)
        {
            case GameState.MainMenu:
                MainMenu();
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
            Warning.SetActive(true);
        }
        else
        {
            Warning.SetActive(false);
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
}
