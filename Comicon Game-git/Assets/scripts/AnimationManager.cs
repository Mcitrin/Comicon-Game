using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {

    public Animator Animator;
    public bool isGrounded;
    public bool chargeing;
    public float speed;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Animate();
    }

    void Animate() {
        // if not grounded play jump animation
        Animator.SetBool("Jump", !isGrounded);

        // if speed > 0 play walking animation
        Animator.SetFloat("Speed", speed);

        // dont play smack animation if were on the ground
        if (isGrounded)
        {
            Animator.SetBool("ChargeSmack", false);
            Animator.SetBool("ChargeSet", chargeing);
        }
        // dont play set animation if were in the air
        else
        {
            Animator.SetBool("ChargeSmack", chargeing);
            Animator.SetBool("ChargeSet", false);
        }

    }
    
    public void setTrigger(string trigger) { 
        Animator.SetTrigger(trigger);
    }

        
}
