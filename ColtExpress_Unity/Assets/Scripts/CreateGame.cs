using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class CreateGame : MonoBehaviour
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
        string address = "http://168.61.46.213:80";
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
            SceneManager.LoadScene ("NewSession");
        }
    }
}
