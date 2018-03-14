using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

    public Animator Animator;
    public bool isGrounded;
    public bool chargeingHit;
    public float speed;


    // Use this for initialization
    void Start()
    {
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }

    void Animate()
    {
        Animator.SetBool("Jump", !isGrounded);

        Animator.SetFloat("Speed", speed);
        Animator.SetBool("Chargeing", chargeingHit);
        

    }

    public void setTrigger(string trigger)
    {
        Animator.SetTrigger(trigger);
    }

    public void hitBall(bool power)
    {
        if (isGrounded)
            Animator.SetTrigger("Set");
        else
        {
            if(power)
            Animator.SetTrigger("PowerSmack");
            else
            Animator.SetTrigger("Smack");
        }
    }


}
