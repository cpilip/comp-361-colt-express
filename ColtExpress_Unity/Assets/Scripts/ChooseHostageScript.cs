using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseHostageScript : MonoBehaviour
{
    // Start is called before the first frame update
    //private int index = 0;
    public Button poodle;
    public Button teacher;
    public Button oldLady;
    public Button photographor;
    public GameObject Panel;
    public GameObject ChooseButton;

    public void Start()
    {
        poodle.onClick.AddListener(() => ButtonClicked(1));
        teacher.onClick.AddListener(() => ButtonClicked(2));
        oldLady.onClick.AddListener(() => ButtonClicked(3));
        photographor.onClick.AddListener(() => ButtonClicked(4));

        
    }

    public void ButtonClicked(int index)
    {
        if (index ==1) {
            Debug.Log("You choose Poodle");
            PlayerPrefs.SetInt("Hostage", 1);
        }else if (index ==2)
        {
            Debug.Log("You choose Teacher");
            PlayerPrefs.SetInt("Hostage", 2);
        }
        else if (index==3)
        {
            Debug.Log("You choose OldLady");
            PlayerPrefs.SetInt("Hostage", 3);
        }
        else
        {
           Debug.Log("You choose Photographor");
            PlayerPrefs.SetInt("Hostage", 4);
        }

        if (index != 0)
        {
            Panel.SetActive(false);
            ChooseButton.SetActive(false);

        }
    }
}
