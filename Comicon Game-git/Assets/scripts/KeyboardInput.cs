using UnityEngine;
using System.Collections;
using InControl;

public class KeyboardInput : MonoBehaviour, IGameInput
{
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
        Vector2 vec = Vector2.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 target = Vector2.zero;

        if (Physics.Raycast(ray, out hit, 100))
        {
            target = new Vector2(hit.point.x, hit.point.y);
            vec = (target - GameObject.Find("Player1New").transform.position).normalized;
        }
        return vec;
    }

    public bool Charge()
    {
        return Input.GetMouseButton(0);
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
