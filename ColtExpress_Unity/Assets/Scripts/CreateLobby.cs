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
        LobbyCommands.registerGameService(this, string.Format("http://127.0.0.1:4243/{0}", name), 
            (int)MaxPlayersSlider.value, 
            (int)MinPlayersSlider.value, 
            name, 
            "true", 
            GameObject.Find("ID").GetComponent<Identification>().getToken());
    }
}
