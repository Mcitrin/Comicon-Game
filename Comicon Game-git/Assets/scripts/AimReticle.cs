using UnityEngine;
using System.Collections;

public class AimReticle : MonoBehaviour
{
    public float Value;
    public GameObject Reticle;
    public GameObject ReticleBorder;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //transform.position =  Vector3.RotateTowards(transform.position, transform.parent.position, 1, 1);


        if (Value > 0)
        {
            Reticle.transform.localScale = new Vector3(Value, Value);
            Reticle.GetComponent<SpriteRenderer>().enabled = true;
            //ReticleBorder.GetComponent<SpriteRenderer>().enabled = true;
            Reticle.GetComponent<SpriteRenderer>().color = new Color(1, 1 - Value, 1 - Value);

        }
        else
        {
            Reticle.transform.localScale = new Vector3(0, 0);
            Reticle.GetComponent<SpriteRenderer>().enabled = false;
            //ReticleBorder.GetComponent<SpriteRenderer>().enabled = false;
        }

    }
}
