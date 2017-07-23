using UnityEngine;
using System.Collections;

public class Appearance : MonoBehaviour {

    public SpriteRenderer HairSprite;
    public SpriteRenderer ShortsSprite;
    public SpriteRenderer StripeSprite;
    public SpriteRenderer arrow;

    Color[] colors = new Color[6];

    // Use this for initialization
    void Start () {
        for (int i = 0; i < 6; i++)
        {
            colors[i] = gameObject.GetComponentsInChildren<SpriteRenderer>()[i].color;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator Flash()
    {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    gameObject.GetComponentsInChildren<SpriteRenderer>()[j].color = Color.red;
                }
                yield return new WaitForSeconds(.025f);
                for (int k = 0; k < 6; k++)
                {
                    gameObject.GetComponentsInChildren<SpriteRenderer>()[k].color = colors[k];
                }
                yield return new WaitForSeconds(.025f);
            }
        

    }
}
