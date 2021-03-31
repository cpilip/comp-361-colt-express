using GameUnitSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getMainCharacter : MonoBehaviour
{
    int getCharacter;
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
                //player.sprite = Tuco;
                NamedClient.c = Character.Tuco;

                Debug.Log("[GetCharacter] You selected Tuco.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Tuco);
                break;
            case 2:
                //player.sprite = Django;
                NamedClient.c = Character.Django;

                Debug.Log("[GetCharacter] You selected Django.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Django);
                break;
            case 3:
                //player.sprite = Ghost;
                NamedClient.c = Character.Ghost;

                Debug.Log("[GetCharacter] You selected Ghost.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Ghost);
                break;
            case 4:
                //player.sprite = Doc;
                NamedClient.c = Character.Doc;

                Debug.Log("[GetCharacter] You selected Doc.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Doc);
                break;
            case 5:
                NamedClient.c = Character.Cheyenne;
               // player.sprite = Che;

                Debug.Log("[GetCharacter] You selected Cheyenne.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Cheyenne);
                break;
            case 6:
                NamedClient.c = Character.Belle;
                //player.sprite = Belle;

                Debug.Log("[GetCharacter] You selected Belle.");
                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(Character.Belle);
                break;
            default:
                Debug.LogError("[GetCharacter] No character selected. Something went wrong!");
                break;

        }

    }
}
