using UnityEngine;
using System.Collections;
using InControl;

public class InputMan : MonoBehaviour {

    private InputDevice joystick1;
    private InputDevice joystick2;
    public bool GotTwoJoysticks;
    public bool NoJoysticks;

    // Use this for initialization
    void Awake () {
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckJoystickCount();
    }

    void CheckJoystickCount()
    {
        if (InputManager.Devices.Count > 0) // if atleast 1 joystick is connected
        {
            joystick1 = InputManager.Devices[0];
            NoJoysticks = false;
        }
        else
        {
            joystick1 = null;
            NoJoysticks = true;
        }
        if (InputManager.Devices.Count > 1) // if 2 joysticks are connected
        {
            joystick2 = InputManager.Devices[1];
            GotTwoJoysticks = true;
        }
        else if (InputManager.Devices.Count <= 1) // if only 1 joystick
        {
            joystick2 = null;
            GotTwoJoysticks = false;
        }
    }

    public bool Jump(int PlayerNum)
    {
        if (PlayerNum == 1)
        {
            if (GotTwoJoysticks) { return joystick2.LeftTrigger; } // if their are two joysticks
            if (Input.GetKeyDown("space")) { return Input.GetKeyDown("space"); }
        }
        if (PlayerNum == 2) { return joystick1.LeftTrigger; }
        return false;
    }

    public float Move(int PlayerNum)
    {
        float result = 0;
        if (PlayerNum == 1)
        {
            if (GotTwoJoysticks) { result += joystick2.LeftStickX; }  // if their are two joysticks
            result += Input.GetAxis("K_Horizontal");
        }
        if (PlayerNum == 2) { result += joystick1.LeftStickX; }
           return Mathf.Clamp(result, -1, 1);
    }

    public Vector2 Aim(int PlayerNum)
    {
        Vector2 vec = Vector2.zero;
        if (PlayerNum == 1)
        {
            if (GotTwoJoysticks)
            {
                vec = new Vector2(joystick2.RightStickX, joystick2.RightStickY).normalized;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Vector3 target = Vector2.zero;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    target = new Vector2(hit.point.x, hit.point.y);
                    vec = (target - GameObject.Find("player1").transform.position).normalized;
                }
            }
        }
        if (PlayerNum == 2)
        {
            vec = new Vector2(joystick1.RightStickX, joystick1.RightStickY).normalized;
        }
            return vec;
        
    }

    public bool Charge(int playerNum)
    {
        if (playerNum == 1)
        {
            if (GotTwoJoysticks){ return joystick2.RightTrigger; }
            else { return Input.GetMouseButton(0); }   
        }
        if(playerNum == 2) { return joystick1.RightTrigger; }
        return false;
    }

}
