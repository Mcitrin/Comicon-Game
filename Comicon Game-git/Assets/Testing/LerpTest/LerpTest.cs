using UnityEngine;

public class LerpTest : MonoBehaviour {

    private Transform pivot;
    
    /// <summary>
    /// Duration, in this case we want this to mean take 1 second for 360 degrees
    /// </summary>
    private float duration = 2f;

    private float t0;
    private float a0 = 0f;
    private float a1 = 0f;    
    private Vector2 joystick;

    // Putting these variables here so I can display them in OnGUI
    private float d = 0f;

    private void Start() {
        pivot = GameObject.Find("Pivot").transform;
        Time.timeScale = 1f;
    }

    private void Update() {
        if (a1 != a0) {

            // Adjust the duration proportional to the angle difference
            d = duration * ( Mathf.Abs(Mathf.DeltaAngle(a0,a1)) / 360f );

            // Current t value (think of it as percentage 0f to 1f)
            float t = (Time.time - t0) / d;

            // Apply easing just cubic, heres the formula graph: 
            // https://www.desmos.com/calculator/iqwdbx269n
            //t = t * Mathf.Pow(t, 2);
            // or
            // https://www.desmos.com/calculator/vp8pp3wqrc
            //t = t * 1 - Mathf.Pow(t - 1, 4);

            // Current angle
            float a = Mathf.LerpAngle(a0, a1, t);

            pivot.localRotation = Quaternion.Euler(0f, 0f, a);
            if (t >= 1f) a0 = a1;
        }
    }

    private void JoystickMoved() {
        t0 = Time.time;
        a0 = pivot.localRotation.eulerAngles.z; // get current, could be mid move
        a1 = Mathf.Atan2(joystick.x, joystick.y) * Mathf.Rad2Deg; 
    }    

#if UNITY_EDITOR
    private void OnGUI() {        
        if(GUI.Button(new Rect(20f,20f,100f,40f), "Move")) {
            
            // Apply random movement (faking joystick here)
            joystick = Random.insideUnitCircle;

            JoystickMoved();

        }
        
        string s = "Joystick: " + joystick + "\n";
        s += "a0=" + a0 + "\n";
        s += "a1=" + a1 + "\n";
        s += "d=" + d + "\n";
        GUI.Label(new Rect(20f, 80f, 300f, 400f), s);
    }
#endif

}
