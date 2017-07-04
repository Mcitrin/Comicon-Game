using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    public int power;
    public Vector3 angle;
    public GameObject arrow;
    public GameObject hand;
    Rigidbody2D rigidbody;
    int arrowDistance = 2;

    float jumpVel = 15.5f;
    bool jumped = false;

    float fallMultiplier = 4.5f;
    float yVelocity;

    float maxVelocity = 7.5f;

    // Use this for initialization
    void OnEnable () {
        rigidbody = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(jumped);
        // pause check
        if (!GameManager.paused)
        {
            if (rigidbody.isKinematic)
            rigidbody.isKinematic = false;
            arrow.transform.position = angle * arrowDistance + this.transform.position;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x,yVelocity);
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
        if(Mathf.Abs(rigidbody.velocity.x) < maxVelocity)
        rigidbody.AddForce(new Vector2(vel,0.0f));
    }

    public void jump(bool jump, bool grounded)
    {

        if (grounded)
        {
            yVelocity = 0;
        }
        if (jump)
        {
            yVelocity = jumpVel;
        }
       

        // jump calculations
        if (rigidbody.velocity.y < 0 && !grounded)
        {
            yVelocity += Physics.gravity.y * (fallMultiplier - 1 *  1.5f) * Time.deltaTime;
        }
        else if (rigidbody.velocity.y > 0)
        {
            yVelocity += Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            
        }
    }

    public IEnumerator setPower(int pow)
    {
        power = pow;
        yield return new WaitForSeconds(.25f);
        power = 0;
    }



    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            CharacterController Player = collision.gameObject.GetComponentInParent<CharacterController>();
            
            if (Player.power == 1 || Player.power == 2)
            {
                Debug.Log(Player.gameObject.name);
                this.gameObject.GetComponentInParent<Rigidbody>().AddForce((transform.position - collision.transform.position) * 100,ForceMode.Impulse);
            }
        }
    }
}
