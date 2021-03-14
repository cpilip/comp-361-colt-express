using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getMainCharacter : MonoBehaviour
{
    public GameObject Bella, Django, Tuco, Ghost, Doc, Cheyenne;
    GameObject myCharacter;
    GameObject clone;
    private readonly string selectCharacter = "SelectedCharacter";

    void Start(){
        int getCharacter;
        //Debug.Log("I'm here");

        getCharacter = PlayerPrefs.GetInt(selectCharacter);
        //Debug.Log("the getCharIndex is: "+getCharacter);
        switch (getCharacter)
        {
            case 1:
                myCharacter = Tuco;
                clone = Instantiate(Tuco);
                Debug.Log("Tuco");
                break;
            case 2:
                myCharacter = Django;
                clone = Instantiate(Django);
                Debug.Log("Django");
                break;
            case 3:
                myCharacter = Ghost;
                clone = Instantiate(Ghost);
                Debug.Log("Ghost");
                break;
            case 4:
                myCharacter = Doc;
                clone = Instantiate(Doc);
                Debug.Log("Doc");
                break;
            case 5:
                myCharacter = Cheyenne;
                clone = Instantiate(Cheyenne);
                Debug.Log("Che");
                break;
            case 6:
                myCharacter = Bella;
                clone = Instantiate(Bella);
                Debug.Log("Bella");
                break;
            default:
                myCharacter = Bella;
                clone = Instantiate(Bella);
                break;

        }
    }

}
