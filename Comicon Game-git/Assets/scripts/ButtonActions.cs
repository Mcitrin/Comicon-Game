using UnityEngine;
using System.Collections;

public class ButtonActions : MonoBehaviour {


    
    PlayManager manager;

	// Use this for initialization
	void Awake () {
        manager = GameManager.gameManager.GetComponent<PlayManager>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void B_Play()
    {
        if (manager.TimeIndex != 0 || manager.ScoreIndex != 0)
        {
            Application.LoadLevel("test");
            manager.gameState = PlayManager.GameState.PlayingGame;
        }

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


}
