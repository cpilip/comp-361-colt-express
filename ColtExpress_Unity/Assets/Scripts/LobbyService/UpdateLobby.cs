using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UpdateLobby : MonoBehaviour
{

    public GameObject Player1Text;
    public GameObject Player2Text;
    public GameObject Player3Text;
    public GameObject Player4Text;
    public GameObject Player5Text;
    public GameObject Player6Text;
    public GameObject SaveGameText;
    public GameObject PingText;

    public Button StartButton;


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

        StartCoroutine(initPlayersWait(0.5f));
        InvokeRepeating("updatePlayersWait", 0.5f, 0.5f);
    }

    void Update()
    {
        // Update all the names of the players currently in the lobby
    }

    public void launchSession(){
        StartCoroutine(launchSessionWait(0.5f));
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
            Debug.Log("LAUNCH GAME!!!!!!!!!!!!!!!!!");
        }
        else
        {
            updateNames(sessInfo.players);
        }
    }

    private IEnumerator launchSessionWait(float time) {
        if (GameObject.Find("sessionId") == null) {
            Debug.Log("we have a problem");
        }

        GameObject sessionId = GameObject.Find("SessionId");
        string id = sessionId.GetComponent<SessionPrefabScript>().getSessionId();

        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.launchSession(this, id, token);
            
        // Call lobby service to delete session
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);
        Debug.Log("LAUNCH GAME!!!!!!!!!!!!!!!!!");
    }

}
