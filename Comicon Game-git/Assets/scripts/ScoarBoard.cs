using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoarBoard : MonoBehaviour
{

    public Numbers player1_score;
    public Numbers player2_score;
    public Numbers time_min;
    public Numbers time_sec;

    public SpriteRenderer p1_score_arrow;
    public SpriteRenderer p2_score_arrow;

    public Canvas popUp;
    public Text popUpText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetNumbers(string number2Reset)
    {
        if (number2Reset == "Score" || number2Reset == "All")
        {
            player1_score.value = 0;
            player2_score.value = 0;
        }
        else if (number2Reset == "Time" || number2Reset == "All")
        {
            time_min.value = 0;
            time_sec.value = 0;
        }
    }

    public void SetNumber(string number2Reset, int value)
    {
        if (number2Reset == "Player1") player1_score.value = value;
        if (number2Reset == "Player2") player2_score.value = value;
        if (number2Reset == "Min") time_min.value = value;
        if (number2Reset == "Sec") time_sec.value = value;
    }

    public IEnumerator PointScored(int playerNum, int winner, int roundCount, bool matchOver)
    {
        SpriteRenderer tmp_score_arrow = null;

        if (playerNum == 1)
        {
            tmp_score_arrow = p1_score_arrow;

        }
        else if (playerNum == 2)
        {
            tmp_score_arrow = p2_score_arrow;

        }

        if (winner == 0)
        {
            popUpText.text = "Point Scored Player " + playerNum;
        }
        else
        {
            if (!matchOver)
            {
                popUpText.text = "Round " + roundCount + " goes to Player " + winner + "!";
            }
            else if(matchOver)
            {
                popUpText.text = "Match goes to Player " + winner + "!!!";
            }
        }

        popUp.enabled = true;
        tmp_score_arrow.enabled = true;
        yield return new WaitForSeconds(2);
        popUp.enabled = false;
        tmp_score_arrow.enabled = false;

    }
}
