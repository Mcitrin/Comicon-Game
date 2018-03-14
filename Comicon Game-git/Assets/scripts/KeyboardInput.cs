using UnityEngine;
using System.Collections;
using InControl;

public class KeyboardInput : MonoBehaviour, IGameInput
{
    // Update is called once per frame
    void Update()
    {
    }

    public bool JumpPress()
    {
        return Input.GetKeyDown("space");
    }

    public bool JumpRelease()
    {
        return Input.GetKeyUp("space");
    }

    public bool Down()
    {
        return Input.GetKey("s");
    }

    public int Move()
    {
        return (int)Input.GetAxis("K_Horizontal");
    }

    public Vector2 MoveVec()
    {
        return Input.mousePosition;
    }


    public Vector2 Aim()
    {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 vec = (target - GameObject.Find("Player1New").transform.position).normalized;
        return vec;
    }

    public bool Charge()
    {
        return Input.GetMouseButton(0);
    }

    public bool ChargeRelease()
    {
        return Input.GetMouseButtonUp(0);
    }

    public bool Enter()
    {
        return Input.GetMouseButton(0);
    }

    public bool Pause()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
