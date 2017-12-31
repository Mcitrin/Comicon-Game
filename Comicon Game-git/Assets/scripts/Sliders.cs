using UnityEngine;
using System.Collections;

public class Sliders : MonoBehaviour {

   public GameObject jumpSlider;
   public GameObject jumpSliderBorder;
   public GameObject hitSlider;
   public GameObject hitSliderBorder;

    public float jumpSliderValue;
    float hitSliderValue;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (jumpSliderValue > 0)
        {
            jumpSlider.transform.localScale = new Vector3(.24f,jumpSliderValue*2);
            jumpSlider.GetComponent<SpriteRenderer>().enabled = true;
            jumpSliderBorder.GetComponent<SpriteRenderer>().enabled = true;
            jumpSlider.GetComponent<SpriteRenderer>().color = new Color(1, 1 - jumpSliderValue, 1 - jumpSliderValue);

        }
        else
        {
            jumpSlider.transform.localScale = new Vector3(.24f, 1);
            jumpSlider.GetComponent<SpriteRenderer>().enabled = false;
            jumpSliderBorder.GetComponent<SpriteRenderer>().enabled = false;
        }

        //if (hitSliderValue > 0)
        //{
        //    hitSlider.transform.localScale = new Vector3(.24f, jumpSliderValue * 2);
        //    hitSlider.GetComponent<SpriteRenderer>().enabled = true;
        //    hitSliderBorder.GetComponent<SpriteRenderer>().enabled = true;
        //    hitSlider.GetComponent<SpriteRenderer>().color = new Color(1, 1 - jumpSliderValue, 1 - jumpSliderValue);
        //
        //}
        //else
        //{
        //    hitSlider.transform.localScale = new Vector3(.24f, 1);
        //    hitSlider.GetComponent<SpriteRenderer>().enabled = false;
        //    hitSliderBorder.GetComponent<SpriteRenderer>().enabled = false;
        //}

    }

    public void SetValue(string slider, float value)
    {
        if(slider == "jump")
        {
            jumpSliderValue = value;
        }
        else if(slider == "hit")
        {
            hitSliderValue = value;
        }

    }
}
