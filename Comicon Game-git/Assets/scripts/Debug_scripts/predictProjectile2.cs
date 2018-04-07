using UnityEngine;
using System.Collections;

public class predictProjectile2 : MonoBehaviour {

    public AIController ai;

	// Use this for initialization
	void Start () {
        transform.position = new Vector2(0, 0);

    }
	
	// Update is called once per frame
	void Update () {

        if(transform.position == Vector3.zero)
        transform.position = ai.tmp;


    }
}
