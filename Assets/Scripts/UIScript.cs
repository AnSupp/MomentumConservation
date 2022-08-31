using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{

    public Button soundButton;

    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private bool sound = true;

    public void SoundSwitcher()
    {
        if (sound)
        {
            soundButton.GetComponent<Image>().sprite = soundOffSprite;
            sound = false;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOnSprite;
            sound = true;
        }
    }
}
