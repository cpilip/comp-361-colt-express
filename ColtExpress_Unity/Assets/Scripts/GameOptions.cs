using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptions : MonoBehaviour
{
    private SelectedButton contentPanel; 
    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    public GameObject GameName;
    public GameObject MinPlayers;
    public GameObject MaxPlayers;
    public GameObject CurrentlyInLobby;
    public GameObject Ping;

    void Start()
    {
       contentPanel = GameObject.Find("Content").GetComponent<SelectedButton>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (contentPanel == null)
        {
            return;
        }

        if (contentPanel.HasChanged){
            StartCoroutine(wait(0.5f));
        }
        
    }

    private IEnumerator wait(float time)
    {
        contentPanel.HasChanged = false;
        GameObject currentButton = contentPanel.Button;
        string currentGame = currentButton.GetComponentInChildren<Text>().text;

        LobbyCommands.getGameService(this, currentGame);

        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        if (response != null){
            GetGameResponse json = JsonUtility.FromJson<GetGameResponse>(response);
            GameName.GetComponent<Text>().text = json.name;
            MinPlayers.GetComponent<Text>().text = json.minSessionPlayers;
            MaxPlayers.GetComponent<Text>().text = json.maxSessionPlayers;
            CurrentlyInLobby.GetComponent<Text>().text = "1/" + json.maxSessionPlayers;
            Ping.GetComponent<Text>().text = "39 ms";
        }
    }
}
