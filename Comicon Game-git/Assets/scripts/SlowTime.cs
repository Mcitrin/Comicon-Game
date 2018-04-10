using UnityEngine;
using System.Collections;

public class SlowTime : MonoBehaviour {

    bool done;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   public void SlowEffect()
    {
        if (!done)
        {
            float normalTime = Time.timeScale;
            Debug.Log(normalTime);
            Time.timeScale = 0;
            Time.timeScale = Linear(0, normalTime, Time.deltaTime);
        }
        done = true;
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * 45);
    }
}
