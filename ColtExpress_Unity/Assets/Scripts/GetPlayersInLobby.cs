using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;


public class GetPlayersInLobby : MonoBehaviour
{

    private bool lobbyLeader = false;
    public Button startButton;
    private string sessionNum;
    private string name; 

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient(); 

    // Start is called before the first frame update
    void Start()
    {
        // Get the session Id for the current Lobby
        GameObject sessionId = GameObject.Find("SessionId");
        if (sessionId == null) {
            Debug.Log("Could not find session Id");
        }

        this.sessionNum = sessionId.GetComponent<SessionPrefabScript>().getId();
        this.name = sessionId.GetComponent<SessionPrefabScript>().name;

        if (!lobbyLeader) {
            startButton.interactable = false;
        }

        getPlayers();

    }

    void Update() {
        if (this.lobbyLeader && !this.startButton.interactable) {
            startButton.interactable = true;
        }
    }

    public void getPlayers() {
        StartCoroutine(getPlayersWait(1));
    }

    private IEnumerator getPlayersWait(float time){
        LobbyCommands.getSessionDetails(this, sessionNum);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);
        SessionInformation sessInfo = JsonConvert.DeserializeObject<SessionInformation>(response);;
        GameObject.Find("GameLister").GetComponent<PlayersInLobby>().setPlayers(sessInfo.players.ToArray()); 

        Debug.Log(sessInfo.creator);

        if (sessInfo.creator.Equals(this.name)) {
            this.lobbyLeader = true;
        }
    }

}
