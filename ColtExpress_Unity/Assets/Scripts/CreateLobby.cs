using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class CreateLobby : MonoBehaviour
{
    public GameObject GameNameText; 
    public Slider MinPlayersSlider;
    public Slider MaxPlayersSlider;

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    public void createGame() {
        StartCoroutine(wait(1));
    }

    private IEnumerator wait(float time)
    {
        string name = GameNameText.GetComponent<TMP_InputField>().text;
        string address = "http://127.0.0.1:4243/DummyGameService";
        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.registerGameService(this, 
            address, 
            (int)MaxPlayersSlider.value, 
            (int)MinPlayersSlider.value, 
            name, 
            "true", 
            token);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);
        if (response == "")
        {
            SceneManager.LoadScene ("Lobby");
        }
    }
}
