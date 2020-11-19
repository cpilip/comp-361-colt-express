using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    public GameObject GameNameText; 
    public Slider MinPlayersSlider;
    public Slider MaxPlayersSlider;

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    public void logIn() {
        //LobbyCommands.signIn(this, GameNameText.GetComponent<TMP_InputField>().text, MinPlayersSlider.value, MaxPlayersSlider.value);
        Debug.Log(LobbyCommands.getResponse());
    }
}
