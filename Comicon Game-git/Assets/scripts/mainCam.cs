using UnityEngine;
using System.Collections;

public class mainCam : MonoBehaviour
{

    float InitalY;
    public Transform top;

    // Use this for initialization
    private void OnEnable()
    {
        InitalY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.gameManager.ball.transform.position.y > top.transform.position.y)
        {
            float yPos = Linear(transform.position.y, GameManager.gameManager.ball.transform.position.y, Time.deltaTime);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }
        else
        {
            float yPos = Linear(transform.position.y, InitalY, Time.deltaTime);
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }


    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * 10);
    }
}
