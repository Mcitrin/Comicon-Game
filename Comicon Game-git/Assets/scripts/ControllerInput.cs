using UnityEngine;
using System.Collections;
using InControl;

public class ControllerInput : MonoBehaviour, IGameInput {

    public
        InputDevice input;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool JumpPress()
    {
        return input.LeftTrigger.WasPressed;
    }

    public bool JumpRelease()
    {
        return input.LeftTrigger.WasReleased;
    }

    public bool Down()
    {
        if (Mathf.Clamp(input.LeftStickX, -1, 1) < 0)
        return true;

        return false;
    }

    public float Move()
    {
        return Mathf.Clamp(input.LeftStickX, -1, 1);
    }

    public Vector2 MoveVec()
    {
        return new Vector2(input.LeftStickX, input.LeftStickY);
    }

    public Vector2 Aim()
    {
        return new Vector2(input.RightStickX, input.RightStickY).normalized;
    }

    public bool Charge()
    {
      return input.RightTrigger;
    }

    public bool Enter()
    {
        return input.Action1;
    }

    public bool Pause()
    {
       
       return input.MenuWasPressed;
    }

}
