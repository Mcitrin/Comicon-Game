using UnityEngine;
using System.Collections;

public class predictProjectile2 : MonoBehaviour {

    public AIController ai;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


        transform.position = ai.tmp;


    }
}
