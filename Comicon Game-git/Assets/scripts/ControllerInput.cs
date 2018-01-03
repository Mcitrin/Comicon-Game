using UnityEngine;
using System.Collections;
using InControl;

public class ControllerInput : MonoBehaviour, IGameInput
{

    public
        InputDevice input;

    // Update is called once per frame
    void Update()
    {
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
        if (Mathf.Clamp(input.LeftStickY, -1, 0) < 0 &&
            (input.LeftStickX <= 0.2f) &&
            (input.LeftStickX >= -0.2f))
            return true;

        return false;
    }

    public int Move()
    {
        return (int)Mathf.Clamp(input.LeftStickX*2, -1, 1);
    }

    public Vector2 MoveVec()
    {
        return new Vector2(input.LeftStickX, input.LeftStickY);
    }

    public Vector2 Aim()
    {
        Vector2 vec = new Vector2(input.RightStickX, input.RightStickY);
        if (vec.magnitude >= .99f)
        {
            return vec.normalized;
        }
        else
        {
            return Vector2.zero;
        }
    }

    public bool Charge()
    {
        return input.RightTrigger;
    }

    public bool ChargeRelease()
    {
        return input.RightTrigger.WasReleased;
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
