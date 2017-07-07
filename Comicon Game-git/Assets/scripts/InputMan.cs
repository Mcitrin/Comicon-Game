using UnityEngine;
using System.Collections;
using InControl;








/*
 
     
fix this class to work with one play using a controler      
     
     
*/





public class InputMan : MonoBehaviour {

    public InputDevice joystick1; // player 2
    public InputDevice joystick2; // player 1 is joystick2 becaus it is only !null if 2 controles are pluged in
    public bool GotTwoJoysticks;
    public bool NoJoysticks;
    public bool onePlayer;

    // Use this for initialization
    void Awake() {
    }

    // Update is called once per frame
    void Update()
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
            if (Input.GetKey("space")) { return Input.GetKey("space"); }
        }
        if (PlayerNum == 2 || onePlayer) { return joystick1.LeftTrigger; }
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
        if (PlayerNum == 2 || onePlayer) { result += joystick1.LeftStickX; }
        return Mathf.Clamp(result, -1, 1);
    }

    public Vector3 MoveVec(int playerNum)
    {
        Vector3 result = Vector3.zero;

        if (playerNum == 1 )
        {
            result.x += joystick2.LeftStickX;
            result.y += joystick2.LeftStickY;
        }
        if (playerNum == 2 || onePlayer)
        {
            result.x += joystick1.LeftStickX;
            result.y += joystick1.LeftStickY;
        }
        return result;

    }

    public Vector2 Aim(int PlayerNum)
    {
        Vector2 vec = Vector2.zero;
        Vector2 velocity = Vector2.zero;
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
                    vec = (target - GameObject.Find("Player1").transform.position).normalized;
                }
            }
        }
        if (PlayerNum == 2 || onePlayer)
        {
            vec = new Vector2(joystick1.RightStickX, joystick1.RightStickY).normalized;
        }
        return vec;
        
        
    }

    public bool Charge(int playerNum)
    {
        if (playerNum == 2 || onePlayer) { return joystick1.RightTrigger; }
        if (playerNum == 1)
        {
            if (GotTwoJoysticks){ return joystick2.RightTrigger; }
            else { return Input.GetMouseButton(0); }   
        }
        
        return false;
    }

    public bool Enter(int playerNum)
    {
        if (playerNum == 1)
         return joystick2.Action1;

        if (playerNum == 2 || onePlayer)
        return joystick1.Action1;

        return false;
    }

    public bool Pause(int playerNum)
    {
        if (playerNum == 1)
        {
            if (GotTwoJoysticks) { return joystick2.MenuWasPressed; }
            else { return Input.GetKeyDown(KeyCode.Escape); }
        }
        if (playerNum == 2 || onePlayer) { return joystick1.MenuWasPressed; }
        return false;
    }

}
