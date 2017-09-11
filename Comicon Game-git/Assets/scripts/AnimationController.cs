using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

    public Animator Animator;
    public bool isGrounded;
    public bool chargeing;
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

        if (isGrounded)
        {
            Animator.SetBool("ChargeSmack", false);
            Animator.SetBool("ChargeSet", chargeing);
        }
        else
        {
            Animator.SetBool("ChargeSmack", chargeing);
            Animator.SetBool("ChargeSet", false);
        }

    }

    public void setTrigger(string trigger)
    {
        Animator.SetTrigger(trigger);
    }


}
