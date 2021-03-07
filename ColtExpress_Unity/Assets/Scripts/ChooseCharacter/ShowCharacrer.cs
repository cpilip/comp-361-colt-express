using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShowCharacrer : MonoBehaviour
{
    int size = 5;
    public GameObject[] characters;
    public GameObject purple;
    public GameObject white;
    public GameObject red;
    public int value = 0;
    //public CharacterSelect tryOne;
    

    public void Start() {
        characters = new GameObject[size];
        characters[0] = purple;
        characters[2] = white;
        characters[1] = red;
        
	//CharacterSelect tryOne = GameObject.Find("ButtonManager").GetComponent<CharacterSelect>();
	//value=tryOne.storeIndex;
        showCharacters();
    }

 


    public void showCharacters() {
	value = PlayerPrefs.GetInt("passValue");
        Debug.Log("The pass result is: "+PlayerPrefs.GetInt("passValue").ToString());
        for (int i = 0; i < 3; i++)
        {
            characters[i].SetActive(false);
        }

        characters[value].SetActive(true);
    }


}
