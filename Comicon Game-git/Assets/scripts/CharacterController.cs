using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    public int power;
    public Vector3 angle;
    public GameObject arrow;
    public GameObject hand;
    Rigidbody rigidbody;
    int arrowDistance = 2;

    float jumpVel = 15.5f;
    bool jumped = false;

    float fallMultiplier = 4.5f;
    Vector3 velocity;

    // Use this for initialization
    void Awake () {
        rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        Debug.Log(jumped);
        // pause check
        if (!GameManager.paused)
        {
            if (rigidbody.isKinematic)
            rigidbody.isKinematic = false;
            arrow.transform.position = angle * arrowDistance + this.transform.position;
            rigidbody.velocity = velocity;
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

    public void Move(float vel)
    {
      velocity.x = vel;
    }

    public void jump(bool jump, bool grounded)
    {
        

        if (jump && !jumped)
        {
            velocity.y = jumpVel;
        }
        else if(grounded)
        {
            velocity.y = 0;
            jumped = false;
        }

        if(!jump && !grounded)
        {
            jumped = true;
        }

        // jump calculations
        if (rigidbody.velocity.y < 0 && !grounded)
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1 *  1.5f) * Time.deltaTime;
        }
        else if (rigidbody.velocity.y > 0 && !jump)
        {
            velocity.y += Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            
        }
    }

    public IEnumerator setPower(int pow)
    {
        power = pow;
        yield return new WaitForSeconds(.25f);
        power = 0;
    }
}
