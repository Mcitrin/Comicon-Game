using UnityEngine;
using System.Collections;

public class predictProjectile : MonoBehaviour {

    Vector2 landingPoint;
    Vector2 V0;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        V0 = GameManager.gameManager.ball.V;

        if (V0 != Vector2.zero)
        {
            if(landingPoint.x != GameManager.gameManager.ball.landingPoint)
            {
                landingPoint = new Vector2(GameManager.gameManager.ball.landingPoint, 0);
            }


            transform.position = landingPoint;
        }
    }
}
