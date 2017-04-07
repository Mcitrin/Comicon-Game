using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pointer : MonoBehaviour
{

    public InputMan inputMan;
    public int PlayerNumber;
    Color color;
    public bool pressed = false;

    // Use this for initialization
    void Start()
    {
        if (inputMan == null)
            inputMan = GameManager.gameManager.GetComponent<InputMan>();
        color = GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckForPress();
    }

    void CheckForPress()
    {
        if (inputMan.Enter(PlayerNumber))
        {
            GetComponent<Image>().color = (color + new Color(.5f, .5f, .5f, .5f));
        }
        else if (!inputMan.Enter(PlayerNumber))
        {
            GetComponent<Image>().color = color;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Slider")// if a slider
        {
            if (inputMan.Enter(PlayerNumber))
            {
                if(other.GetComponentInParent<CharacterCustomization>().playerNum == 2)
                other.GetComponentInParent<Slider>().normalizedValue -= (inputMan.MoveVec(PlayerNumber).x / 10);
                else
                other.GetComponentInParent<Slider>().normalizedValue += (inputMan.MoveVec(PlayerNumber).x / 10);
                pressed = true;
            }
            else if (!inputMan.Enter(PlayerNumber))
            {
                pressed = false;
            }
        }
        else // els if not a slider
        {
            if (inputMan.Enter(PlayerNumber) && !pressed)
            {
                other.GetComponent<Button>().onClick.Invoke();
                pressed = true;
            }
            else if (!inputMan.Enter(PlayerNumber) && pressed)
            {
                pressed = false;
            }
        }
    }

    void Move()
    {
        if (!inputMan.NoJoysticks)
            transform.position += inputMan.MoveVec(PlayerNumber) * 5;
    }
}
