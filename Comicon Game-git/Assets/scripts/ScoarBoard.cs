using UnityEngine;
using System.Collections;

public class ScoarBoard : MonoBehaviour {

    public Numbers player1_score;
    public Numbers player2_score;
    public Numbers time_min;
    public Numbers time_sec;

    public SpriteRenderer p1_score_arrow;
    public SpriteRenderer p2_score_arrow;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ResetNumbers(string number2Reset)
    {
        if(number2Reset == "Score" || number2Reset == "All")
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

    public IEnumerator PointScored(int playerNum)
    {
        SpriteRenderer tmp_score_arrow = null;

        if (playerNum == 1) tmp_score_arrow = p1_score_arrow;
        else if (playerNum == 2) tmp_score_arrow = p2_score_arrow;

        tmp_score_arrow.enabled = true;
        yield return new WaitForSeconds(2);
        tmp_score_arrow.enabled = false;

    }
}
