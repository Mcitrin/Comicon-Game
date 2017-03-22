﻿using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

   public Color HairColor_P1;
   public Color ShortsColor_P1;
   public Color StripeColor_P1;
   public Color HairColor_P2;
   public Color ShortsColor_P2;
   public Color StripeColor_P2;

    public int number;

    private void Awake()
    {
        number = GameObject.FindObjectsOfType<PlayerInfo>().Length;
        DontDestroyOnLoad(gameObject);
        if (number > 1)
        Destroy(gameObject); 
    }
        // Use this for initialization


    void OnLevelWasLoaded()
    {
        if (GameObject.Find("Player1_UI") != null && GameObject.Find("Player2_UI") != null)
        {
            Debug.Log(HairColor_P1);
            SetCustomizerColors();
        }
        if(GameObject.Find("Player1") != null && GameObject.Find("Player2") != null)
        SetPlayersColors();
    }
	
	// Update is called once per frame
	void Update () {
      
            SetPlayerInfoColors();
    }

    void SetCustomizerColors() // sets the colers of the customizers to that of the players info
    {
            //p1
            GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().HairColor = HairColor_P1;
            GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().ShortsColor = ShortsColor_P1;
            GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().StripeColor = StripeColor_P1;

        GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().Red.value = HairColor_P1.r;
        GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().Green.value = HairColor_P1.g;
        GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().Blue.value = HairColor_P1.b;

        //p2
        GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().HairColor = HairColor_P2;
            GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().ShortsColor = ShortsColor_P2;
            GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().StripeColor = StripeColor_P2;

        GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().Red.value = HairColor_P2.r;
        GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().Green.value = HairColor_P2.g;
        GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().Blue.value = HairColor_P2.b;
    }

        void SetPlayerInfoColors() // set the player infos coloers to that of the customizer
    {

            if (GameObject.Find("Player1_UI") != null)
            {
                HairColor_P1 = GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().HairColor;
                ShortsColor_P1 = GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().ShortsColor;
                StripeColor_P1 = GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().StripeColor;
            }
            if (GameObject.Find("Player2_UI") != null)
            {
                HairColor_P2 = GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().HairColor;
                ShortsColor_P2 = GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().ShortsColor;
                StripeColor_P2 = GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().StripeColor;
            }
     }
    void SetPlayersColors() // sets the colors of the players themselfs
    {
        //p1
        GameObject.Find("Player1").GetComponent<Move>().Hair.color = HairColor_P1;
        GameObject.Find("Player1").GetComponent<Move>().Shorts.color = ShortsColor_P1;
        GameObject.Find("Player1").GetComponent<Move>().Stripe.color = StripeColor_P1;
        //p2
        GameObject.Find("Player2").GetComponent<Move>().Hair.color = HairColor_P2;
        GameObject.Find("Player2").GetComponent<Move>().Shorts.color = ShortsColor_P2;
        GameObject.Find("Player2").GetComponent<Move>().Stripe.color = StripeColor_P2;
    }
}