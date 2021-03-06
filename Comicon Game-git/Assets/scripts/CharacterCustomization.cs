﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterCustomization : MonoBehaviour {
    //buttons
    public Button HairButton;
    public Button ShortsButton;
    public Button StripeButton;
    //sliders
    public Slider Red;
    public Slider Green;
    public Slider Blue;
    //images
    public Image HairImage;
    public Image ShortsImage;
    public Image StripeImage;
    //colors
    public Color HairColor;
    public Color ShortsColor;
    public Color StripeColor;
    //misc
    public int playerNum;
    enum State
    {
       Hair,
       Shorts,
       Stripe,
    };
     State state = State.Hair;


    // Use this for initialization
    void Start () {
        HairButton.GetComponent<Image>().color = Color.yellow;
        ShortsButton.GetComponent<Image>().color = Color.white;
        StripeButton.GetComponent<Image>().color = Color.white;
    }

    public void Hair()
    {
       state = State.Hair;
        Red.value = HairColor.r;
        Blue.value = HairColor.b;
        Green.value = HairColor.g;

        HairButton.GetComponent<Image>().color = Color.yellow;
        ShortsButton.GetComponent<Image>().color = Color.white;
        StripeButton.GetComponent<Image>().color = Color.white;
    }

    public void Shorts()
    {
        state = State.Shorts;
        Red.value = ShortsColor.r;
        Blue.value = ShortsColor.b;
        Green.value = ShortsColor.g;

        HairButton.GetComponent<Image>().color = Color.white;
        ShortsButton.GetComponent<Image>().color = Color.yellow;
        StripeButton.GetComponent<Image>().color = Color.white;
    }

    public void Stripe()
    {
        state = State.Stripe;
        Red.value = StripeColor.r;
        Blue.value = StripeColor.b;
        Green.value = StripeColor.g;

        HairButton.GetComponent<Image>().color = Color.white;
        ShortsButton.GetComponent<Image>().color = Color.white;
        StripeButton.GetComponent<Image>().color = Color.yellow;
    }

    public void RandomizeColors()
    {

        Vector3 hairColors = new Vector3(Random.value, Random.value, Random.value);
        HairColor = new Color(hairColors.x, hairColors.y, hairColors.z);

        Vector3 ShortColors = new Vector3(Random.value, Random.value, Random.value);
        ShortsColor = new Color(ShortColors.x, ShortColors.y, ShortColors.z);

        Vector3 stripeColors = new Vector3(Random.value, Random.value, Random.value);
        StripeColor = new Color(stripeColors.x, stripeColors.y, stripeColors.z);

        // hacky bullshit
        switch (state)
        {
            case State.Hair:
                Red.value = hairColors.x;
                Blue.value = hairColors.y;
                Green.value = hairColors.z;
                break;
            case State.Shorts:
                Red.value = ShortColors.x;
                Blue.value = ShortColors.y;
                Green.value = ShortColors.z;
                ShortsImage.color = ShortsColor;
                break;
            case State.Stripe:
                Red.value = stripeColors.x;
                Blue.value = stripeColors.y;
                Green.value = stripeColors.z;
                break;
        }
    }

    // Update is called once per frame
    void Update () {
        HairImage.color = HairColor;
        ShortsImage.color = ShortsColor;
        StripeImage.color = StripeColor;


        switch (state)
        {
            case State.Hair:
                HairColor = new Color(Red.value, Green.value, Blue.value);
                Red.value = HairColor.r;
                Blue.value = HairColor.b;
                Green.value = HairColor.g;
                HairImage.color = HairColor;
                break;
            case State.Shorts:
                ShortsColor = new Color(Red.value, Green.value, Blue.value);
                Red.value = ShortsColor.r;
                Blue.value = ShortsColor.b;
                Green.value = ShortsColor.g;
                ShortsImage.color = ShortsColor;
                break;
            case State.Stripe:
                StripeColor = new Color(Red.value, Green.value, Blue.value);
                Red.value = StripeColor.r;
                Blue.value = StripeColor.b;
                Green.value = StripeColor.g;
                StripeImage.color = StripeColor;
                break;
        }
	
	}
}
