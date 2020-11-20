using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateLobby : MonoBehaviour
{
    public GameObject GameNameText; 
    public Slider MinPlayersSlider;
    public Slider MaxPlayersSlider;

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    public void createGame() {
        string name = GameNameText.GetComponent<TMP_InputField>().text;
        string address = string.Format("http://127.0.0.1:4243/{0}", name);
        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.registerGameService(this, 
            address, 
            (int)MaxPlayersSlider.value, 
            (int)MinPlayersSlider.value, 
            name, 
            "true", 
            token);
        Debug.Log(LobbyCommands.getResponse());
    }
}
