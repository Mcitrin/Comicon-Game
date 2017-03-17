using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   
    public void SetBool(Animator anim, string name, float time)
    {
        anim.SetBool(name, true);
        StartCoroutine(PlayAnimation(anim, name, time));
    }

    IEnumerator PlayAnimation(Animator anim, string name, float time)
    {
        
        yield return new WaitForSeconds(time);
        anim.SetBool(name, false);
    }
}
