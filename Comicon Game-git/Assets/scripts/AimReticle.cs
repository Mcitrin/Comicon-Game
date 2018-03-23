using UnityEngine;
using System.Collections;

public class AimReticle : MonoBehaviour
{
    public float Value;
    public Animator fill;

    // Use this for initialization
    void Start () {
        //fill.clip.legacy = true;
    }
	
	// Update is called once per frame
	void Update () {
        //transform.position =  Vector3.RotateTowards(transform.position, transform.parent.position, 1, 1);

        //Debug.Log(fill.animation.name);

        if (Value > 0)
        {
            //Reticle.transform.localScale = new Vector3(Value, Value);
            //Reticle.GetComponent<SpriteRenderer>().enabled = true;
            ////ReticleBorder.GetComponent<SpriteRenderer>().enabled = true;
            //Reticle.GetComponent<SpriteRenderer>().color = new Color(1, 1 - Value, 1 - Value);

            fill.ForceStateNormalizedTime(Value);
        }
        else
        {
            //Reticle.transform.localScale = new Vector3(0, 0);
            //Reticle.GetComponent<SpriteRenderer>().enabled = false;
            //ReticleBorder.GetComponent<SpriteRenderer>().enabled = false;

            fill.ForceStateNormalizedTime(0);


        }

    }
}
