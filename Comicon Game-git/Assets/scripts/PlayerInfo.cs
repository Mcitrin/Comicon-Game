using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{

    public static PlayerInfo playerInfo;

    public Color HairColor_P1;
    public Color ShortsColor_P1;
    public Color StripeColor_P1;
    public Color HairColor_P2;
    public Color ShortsColor_P2;
    public Color StripeColor_P2;

    public int P1Wins = 0;
    public int P2Wins = 0;

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
        if (number == 1)
        {
            if (GameObject.Find("Player1_UI") != null && GameObject.Find("Player2_UI") != null)
            {
                SetCustomizerColors();
                Debug.Log("SetCustomizerColors();");
            }

            if (GameObject.Find("Player1") != null && GameObject.Find("Player2") != null)
            {
                SetPlayersColors();
                Debug.Log("SetPlayersColors();");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCustomizerColors() // sets the colers of the customizers to that of the players info
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

    public void SetPlayerInfoColors() // set the player infos coloers to that of the customizer
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
    public void SetPlayersColors() // sets the colors of the players themselfs
    {
        if (GameObject.Find("Player1") != null && GameObject.Find("Player2") != null)
        {
            //p1
            GameObject.Find("Player1").GetComponent<Move>().HairSprite.color = HairColor_P1;
            GameObject.Find("Player1").GetComponent<Move>().ShortsSprite.color = ShortsColor_P1;
            GameObject.Find("Player1").GetComponent<Move>().StripeSprite.color = StripeColor_P1;
            GameObject.Find("Player1").GetComponent<Move>().arrow.GetComponent<SpriteRenderer>().color = ShortsColor_P1;
            //p2
            GameObject.Find("Player2").GetComponent<Move>().HairSprite.color = HairColor_P2;
            GameObject.Find("Player2").GetComponent<Move>().ShortsSprite.color = ShortsColor_P2;
            GameObject.Find("Player2").GetComponent<Move>().StripeSprite.color = StripeColor_P2;
            GameObject.Find("Player2").GetComponent<Move>().arrow.GetComponent<SpriteRenderer>().color = ShortsColor_P2;
        }
    }

    bool ColorsCorrect(string mode)
    {
        Debug.Log(mode);
        if (mode == "menu")
        {
            if (
                //p1
                GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().HairColor == HairColor_P1 &&
                GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().ShortsColor == ShortsColor_P1 &&
                GameObject.Find("Player1_UI").GetComponent<CharacterCustomization>().StripeColor == StripeColor_P1 &&

                //p2
                GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().HairColor == HairColor_P2 &&
                GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().ShortsColor == ShortsColor_P2 &&
                GameObject.Find("Player2_UI").GetComponent<CharacterCustomization>().StripeColor == StripeColor_P2
                )
                return true;
        }
        else if (mode == "Play")
        {
            if(
            //p1
            GameObject.Find("Player1").GetComponent<Move>().HairSprite.color == HairColor_P1 &&
            GameObject.Find("Player1").GetComponent<Move>().ShortsSprite.color == ShortsColor_P1 &&
            GameObject.Find("Player1").GetComponent<Move>().StripeSprite.color == StripeColor_P1 &&
            //p2
            GameObject.Find("Player2").GetComponent<Move>().HairSprite.color == HairColor_P2 &&
            GameObject.Find("Player2").GetComponent<Move>().ShortsSprite.color == ShortsColor_P2 &&
            GameObject.Find("Player2").GetComponent<Move>().StripeSprite.color == StripeColor_P2
            )
            return true;
        }
            return false;
    }
}
