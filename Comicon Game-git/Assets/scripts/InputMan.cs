using UnityEngine;
using System.Collections;
using InControl;

public class InputMan : MonoBehaviour {

    private InputDevice joystick1;
    private InputDevice joystick2;


    // Use this for initialization
    void Awake () {
    }
	
	// Update is called once per frame
	void Update () {
        if(InputManager.Devices.Count > 0)joystick1 = InputManager.Devices[0];
        if (InputManager.Devices.Count > 1) joystick2 = InputManager.Devices[1];
    }

    public bool Jump1()
    {
        if (joystick2 != null)
            return joystick2.LeftTrigger;
        else if (Input.GetKeyDown("space"))
            return Input.GetKeyDown("space");
        else
            return false;
    }

    public float Move1()
    {
        float result = 0;
        if (joystick2 != null)
        {
            result += joystick2.LeftStickX;
        }
        result += Input.GetAxis("K_Horizontal");

        return Mathf.Clamp(result, -1, 1);
    }

    public Vector2 Aim1()
    {
        Vector2 vec = Vector2.zero;

        if(joystick2!=null)
        vec = new Vector2(joystick2.RightStickX, joystick2.RightStickY).normalized;  
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Vector3 target = Vector2.zero;

            if (Physics.Raycast(ray, out hit, 100))
            {
                target = new Vector2(hit.point.x,hit.point.y);
                vec = (target - GameObject.Find("player1").transform.position).normalized;
            }
            Debug.Log(vec);
        }

        return vec;
        
    }

    public bool Jump2()
    {
        return joystick1.LeftTrigger;
    }

    public float Move2()
    {
        float result = 0;
        result += joystick1.LeftStickX;
        return Mathf.Clamp(result, -1, 1);
    }

    public Vector2 Aim2()
    {
           return new Vector2(joystick1.RightStickX, joystick1.RightStickY).normalized;
    }
}
