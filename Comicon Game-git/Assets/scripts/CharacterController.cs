using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    public int power;
    public Vector3 angle;
    public GameObject arrow;
    public GameObject hand;
    Rigidbody rigidbody;
    int arrowDistance = 2;

    // Use this for initialization
    void Awake () {
        rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
       
        // pause check
        if (!GameManager.paused)
        {
            if (rigidbody.isKinematic)
            rigidbody.isKinematic = false;
            arrow.transform.position = angle * arrowDistance + this.transform.position;
        }
        else
        {
            if (!rigidbody.isKinematic)
            rigidbody.isKinematic = true;
            rigidbody.velocity = Vector3.zero;
        }

        
    }

    public void Aim(Vector3 arrowAngle)
    {
        angle = arrowAngle; 
    }

    public void Move(Vector3 velocity)
    {
    rigidbody.velocity = velocity;
    }

    public void jump(Vector3 jumpVelocity)
    {
    rigidbody.velocity += jumpVelocity;
    }

    public IEnumerator setPower(int pow)
    {
        power = pow;
        yield return new WaitForSeconds(.25f);
        power = 0;
    }
}
