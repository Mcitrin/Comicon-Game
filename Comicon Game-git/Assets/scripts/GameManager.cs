using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;

    // Use this for initialization
    void Awake () {
        if(gameManager == null) { gameManager = this; }
        DontDestroyOnLoad(gameObject);
        Random.seed = (int)System.DateTime.Now.Ticks;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
