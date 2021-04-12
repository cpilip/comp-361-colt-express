using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class UpdateLobby : MonoBehaviour
{

    GameObject Player1Text;
    GameObject Player2Text;
    GameObject Player3Text;
    GameObject Player4Text;
    GameObject Player5Text;
    GameObject Player6Text;
    GameObject SaveGameText;
    GameObject PingText;

    Button StartButton;


    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();
    private List<Text> playerTexts;

    void Start()
    {
        // Initialize all the text updates to easily update them
        playerTexts = new List<Text>();
        playerTexts.Add(Player1Text.GetComponent<Text>());
        playerTexts.Add(Player2Text.GetComponent<Text>());
        playerTexts.Add(Player3Text.GetComponent<Text>());
        playerTexts.Add(Player4Text.GetComponent<Text>());
        playerTexts.Add(Player5Text.GetComponent<Text>());
        playerTexts.Add(Player6Text.GetComponent<Text>());

        StartCoroutine(initPlayersWait());
    }

    void Update()
    {
        // Update all the names of the players currently in the lobby
    }

    public void launchSession(){

    }

    private IEnumerator initPlayersWait(float time)
    {
        string sessionID = GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().getSessionId();
        bool creator = GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().getCreator();

        if (!creator)
        {
            StartButton.interactable = false;
        }

        // Call lobby service to create session
        LobbyCommands.getSession(this, sessionID);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        // Parse the response from the server
        SessionInformation sessInfo = JObject.Parse(response).ToObject<SessionInformation>();

        SaveGameText.GetComponent<Text>().text = "Using savegame: " + sessInfo.savegameid;
        StartCoroutine(StartPing("74.125.224.72"));
        updateNames(sessInfo.players);
    }

    private void updateNames(List<string> names) {
        for (int i = 0 ; i < names.Count ; i++) {
            playerTexts[i].text = names[i];
        }
    }

    IEnumerator StartPing(string ip)
    {
        WaitForSeconds f = new WaitForSeconds(0.05f);
        Ping p = new Ping(ip);
        while (p.isDone == false)
        {
            yield return f;
        }
        Debug.Log("Ping is done   " + p.time);
        PingFinished(p);
    }


    private void PingFinished(Ping p)
    {
        Text pText = PingText.GetComponent<Text>();
        pText.text = "" + p.time + " ms";
    }

    private IEnumerator updatePlayersWait(float time)
    {
        bool creator = GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().getCreator();
        string sessionID = GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().getSessionId();

        // Call lobby service to create session
        LobbyCommands.getSession(this, sessionID);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        // Parse the response from the server
        SessionInformation sessInfo = JObject.Parse(response).ToObject<SessionInformation>();

        if (sessInfo.launched)
        {
            // LAUNCH THE GAME HERE
        }
        else
        {
            updateNames(sessInfo.players);
        }
    }

    private IEnumerator launchSession(float time) {

    }

}
