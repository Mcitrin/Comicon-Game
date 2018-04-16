using UnityEngine;
using System.Collections;
using InControl;








/*
 
     
fix this class to work with one play using a controler      
     
     
*/
public interface IGameInput
{
    bool JumpPress();
    bool JumpRelease();
    bool Down();
    int Move();
    Vector2 MoveVec();
    Vector2 Aim();
    bool Charge();
    bool ChargeRelease();
    bool Enter();
    bool Pause();
}


public class InputMan : MonoBehaviour {



    public InputDevice joystick1; // player 2 is joystick1 unless theirs only one player in wich case they use joystick 1
    public InputDevice joystick2; // player 1 is joystick2 if their are two controlers pluged in otherwise they use mouse and keyboard
    public static KeyboardInput keyboard; // refrenc to the keyboard input component
    public static ControllerInput controller; // refrence to the conrtollere input component
    public bool GotTwoJoysticks; // do we have 2 joysticks
    public bool NoJoysticks;// do we have no josticks

    public InputDevice inputDevice; // this is assigned in CheckJoystickCount method
    public IGameInput input = keyboard; // default is keyboard, also gets changed in the CheckJoystickCount method


    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<KeyboardInput>();
        gameObject.AddComponent<ControllerInput>();
        keyboard = GetComponent<KeyboardInput>();
        controller = GetComponent<ControllerInput>();
        StartCoroutine(CheckJoystickCountCo());
    }
    IEnumerator CheckJoystickCountCo()
    {
        while (true)
        {
            CheckJoystickCount();
            yield return new WaitForSeconds(1f);
        }
    }

    void CheckJoystickCount()
    {
        // seets joystick refrences and jpystick count bools

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

    IGameInput GetInputDevice(int playerNum)
    {
        // if your player 1 and theirs 2 joysticks pluged
        if (playerNum == 1 && InputManager.Devices.Count > 1)
        {
            controller.input = joystick2;
            return controller;
        }

        // if your player 1 and theirs 1 joystick pluged in and player 2 is an AI
       else if (playerNum == 1 && InputManager.Devices.Count == 1 && GameManager.gameManager.player2IsAI)
        {
            controller.input = joystick1;
            return controller;
        }

        // if your player 1 and player 2 is not an AI and theirs only 1 controller
        else if (playerNum == 1 && (NoJoysticks || !GameManager.gameManager.player2IsAI))
        {
            return keyboard;
        }

        // if your player 2 and thiers at least one joystick
        else if (playerNum == 2)
        {
            controller.input = joystick1;
            return controller;
        }

        Debug.Log("null");
        return null;
    }

    public bool JumpPress(int playerNum)
    {
        return GetInputDevice(playerNum).JumpPress();
    }

    public bool JumpRelease(int playerNum)
    {
        return GetInputDevice(playerNum).JumpRelease();
    }


    public int Move(int playerNum)
    {
        return GetInputDevice(playerNum).Move();
    }

    public Vector3 MoveVec(int playerNum)
    {
        return GetInputDevice(playerNum).MoveVec();
    }

    public Vector2 Aim(int playerNum)
    {
        //Debug.Log(GetInputDevice(playerNum).Aim());
        return GetInputDevice(playerNum).Aim();
    }

    public bool Charge(int playerNum)
    {
        return GetInputDevice(playerNum).Charge();
    }

    public bool ChargeRelease(int playerNum)
    {
        return GetInputDevice(playerNum).ChargeRelease();
    }

    public bool Enter(int playerNum)
    {
        return GetInputDevice(playerNum).Enter();

    }

    public bool Pause()
    {
        if(InputManager.Devices.Count < 1)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                return true;
            }
        }
        else if(InputManager.Devices.Count == 1)
        {
            if (joystick1.MenuWasPressed || Input.GetKeyDown(KeyCode.Escape))
            {
                return true;
            }
        }
        else
        {
            if (joystick1.MenuWasPressed || joystick2.MenuWasPressed || Input.GetKeyDown(KeyCode.Escape))
            {
                return true;
            }
        }
            return false;
    }

    public bool Down(int playerNum)
    {
        return GetInputDevice(playerNum).Down();
    }

}
