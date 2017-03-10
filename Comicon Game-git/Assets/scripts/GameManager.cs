using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;

    // Use this for initialization
    void Awake () {
        DontDestroyOnLoad(gameObject);
        if(gameManager == null) { gameManager = this; }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
