using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonActions : MonoBehaviour {


    
    PlayManager manager;
   public PlayerInfo playInfo;

	// Use this for initialization
	/*void Awake () {
        //manager = null;//GameManager.gameManager.GetComponent<PlayManager>();
    }
	
	// Update is called once per frame
	void Update () {
        if (playInfo == null) { playInfo = manager.playerInfo; }
    }
    public void B_Play()
    {
        if (manager.TimeIndex != 0 || manager.ScoreIndex != 0)
        {
            if (!manager.inputMan.NoJoysticks)
            {
                playInfo.SetPlayerInfoColors();
                Application.LoadLevel("test");
                manager.gameState = PlayManager.GameState.SetUp;
            }
        }

    }

    public void B_Menu()
    {
        Application.LoadLevel("Menu");
        Destroy(manager.gameObject);
    }
    public void B_Quit()
    {
        Application.Quit();
    }
    public void B_TimeL()
    {
        if (manager.TimeIndex != 0)
            manager.TimeIndex--;
        else
            manager.TimeIndex = 4;
    }
    public void B_TimeR()
    {
        if (manager.TimeIndex != 4)
            manager.TimeIndex++;
        else
            manager.TimeIndex = 0;
    }
    public void B_ScoreL()
    {
        if (manager.ScoreIndex != 0)
            manager.ScoreIndex--;
        else
            manager.ScoreIndex = 4;
    }
    public void B_ScoreR()
    {
        if (manager.ScoreIndex != 4)
            manager.ScoreIndex++;
        else
            manager.ScoreIndex = 0;
    }
    */
    public void B_Player1Switch()
    {
        GameObject obj = GameObject.Find("Player1_ControllerSwitch");

        if (!GameManager.gameManager.player1IsAI)
        {
            obj.GetComponentInChildren<Text>().text = "Switch\nPlayer1: AI";
        }
        else if (GameManager.gameManager.player1IsAI)
        {
            obj.GetComponentInChildren<Text>().text = "Switch\nPlayer1: Player";
        }
        GameManager.gameManager.ChangePlayerController(1);
    }

    public void B_Player2Switch()
    {
        GameObject obj = GameObject.Find("Player2_ControllerSwitch");

        if (!GameManager.gameManager.player2IsAI)
        {
            obj.GetComponentInChildren<Text>().text = "Switch\nPlayer2: AI";
        }
        else if (GameManager.gameManager.player2IsAI)
        {
            obj.GetComponentInChildren<Text>().text = "Switch\nPlayer2: Player";
        }
        GameManager.gameManager.ChangePlayerController(2);
    }


}
