using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

    InputMan input;
    float distToGround;

    // Use this for initialization
    void Awake()
    {
        input = GameManager.gameManager.GetComponent<InputMan>();
        distToGround = GetComponent<BoxCollider>().bounds.extents.y;
    }
    
    // Update is called once per frame
    void Update () {
	if(input.Jump() && IsGrounded())
        {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0, 7.5f, 0);//7.5f;
        }
	}

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
