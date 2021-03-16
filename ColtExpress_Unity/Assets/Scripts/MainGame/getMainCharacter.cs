using GameUnitSpace;
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
    public Sprite Belle;
    public Image player;
    private readonly string selectCharacter = "SelectedCharacter";

    void Start() {

        getCharacter = PlayerPrefs.GetInt(selectCharacter);
        //Debug.Log("the getCharIndex is: "+getCharacter);
        switch (getCharacter)
        {
            case 1:
                player.sprite = Tuco;
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Tuco);
        	    Debug.Log("Tuco");
                break;
            case 2:
                player.sprite = Django;
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Django);
                Debug.Log("Tuco");
                break;
            case 3:
                player.sprite = Ghost;
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Ghost);
                Debug.Log("Ghost");
                break;
            case 4:
                player.sprite = Doc;
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Doc);
                Debug.Log("Doc");
                break;
            case 5:
                player.sprite = Che;
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Cheyenne);
                Debug.Log("Che");
                break;
            case 6:
                player.sprite = Belle;
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Belle);
                Debug.Log("Bella");
                break;
            default:
                player.sprite = Belle;
                break;

        }

    }

    public void setSprite(Character c)
    {
        switch (c)
        {
            case Character.Tuco:
                player.sprite = Tuco;
                break;
            case Character.Django:
                player.sprite = Django;
                break;
            case Character.Ghost:
                player.sprite = Ghost;
                break;
            case Character.Doc:
                player.sprite = Doc;
                break;
            case Character.Cheyenne:
                player.sprite = Che;
                break;
            case Character.Belle:
                player.sprite = Belle;
                break;
            default:
                player.sprite = Belle;
                break;

        }
    }




}
