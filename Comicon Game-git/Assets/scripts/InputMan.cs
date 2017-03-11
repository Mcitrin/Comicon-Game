using UnityEngine;
using System.Collections;
using InControl;

public class InputMan : MonoBehaviour {

    private InputDevice joystick;

	// Use this for initialization
	void Awake () {
        joystick = InputManager.ActiveDevice;
	}
	
	// Update is called once per frame
	void Update () {
        if (joystick == null) { joystick = InputManager.ActiveDevice; }
	}

    public bool Jump()
    {
        if (joystick.LeftTrigger)
            return joystick.LeftTrigger;
        else if (Input.GetKeyDown("space"))
            return Input.GetKeyDown("space");
        else
            return false;
    }
}
