using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getMainCharacter : MonoBehaviour
{
    //GameObject myCharacter;
    //GameObject clone;
    int getCharacter;
    //Debug.Log("I'm here");
    public Sprite Tuco;
    public Sprite Django;
    public Sprite Ghost;
    public Sprite Doc;
    public Sprite Che;
    public Sprite Bella;
    public Image player;
    private readonly string selectCharacter = "SelectedCharacter";

    void Start() {

        getCharacter = PlayerPrefs.GetInt(selectCharacter);
        //Debug.Log("the getCharIndex is: "+getCharacter);
        switch (getCharacter)
        {
            case 1:
                player.sprite = Tuco;
        	Debug.Log("Tuco");
                break;
            case 2:
                player.sprite = Django;
        	Debug.Log("Tuco");
                break;
            case 3:
                player.sprite = Ghost;
                Debug.Log("Ghost");
                break;
            case 4:
                player.sprite = Doc;
                Debug.Log("Doc");
                break;
            case 5:
                player.sprite = Che;
                Debug.Log("Che");
                break;
            case 6:
                player.sprite = Bella;
                Debug.Log("Bella");
                break;
            default:
                player.sprite = Bella;
                break;

        }

    }



}
