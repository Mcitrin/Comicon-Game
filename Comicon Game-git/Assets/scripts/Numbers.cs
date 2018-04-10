using UnityEngine;
using System.Collections;

public class Numbers : MonoBehaviour {


    Animator animator;
    public float value;
    float ones;
    float tens;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {


        ones = (value % 10);
        tens = ((value % 100 - ones)/10);

        //animator.ForceStateNormalizedTime(value/10f);
        animator.Play("Count", 0, (ones/10));
        animator.Play("Count_tens", 1, (tens/10));

    }
}
