using UnityEngine;
using System.Collections;

public class Lerper : MonoBehaviour {

    float Duration;
    float A0;
    float A1;
    float T0;

   public bool lerping = false;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(lerping)
        {
            if ( T0+Duration > Time.time)
            {
                float d = Duration * (Mathf.Abs(A1 - A0) / 100);
                float t = (Time.time - T0) / Duration;

               if(A0 > A1)
               t = t * Mathf.Pow(t, 2);
               
               if(A0 < A1)
               t = t * 1 - Mathf.Pow(t - 1, 4);

                float a = Mathf.Lerp(A0, A1, t);
                transform.position = new Vector2(transform.position.x, a);
            }
            else
            {
                Stop();
            }
        }

	}

    public void SetUpLerp(float a0, float a1, float duration, bool lerp)
    {
        Debug.Log("setUp");
        lerping = lerp;
        Duration = duration;
        A0 = a0;
        A1 = a1;
        T0 = Time.time;
    }

    public void Stop()
    {
        SetUpLerp(0, 0, 0, false);
    }
}
