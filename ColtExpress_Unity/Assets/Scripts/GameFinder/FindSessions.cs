using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;



public class FindSessions : MonoBehaviour
{
    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    List<string> gameSessions;
    public Dropdown sessionsDropDown;

    public Object SessionPrefab; 

    // Start is called before the first frame update
    void Start()
    {
        findGames();
    }

    public void findGames() {
        StartCoroutine(findGamesWait(1));
    }

    private IEnumerator findGamesWait(float time){
        LobbyCommands.getSessions(this);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        SessionsInformation responseParsed = JsonConvert.DeserializeObject<SessionsInformation>(response);

        Debug.Log(responseParsed.sessions);

        List<string> currentSessions = new List<string>();
        foreach (string sessInfo in responseParsed.sessions.Keys) {
            currentSessions.Add(sessInfo);
        }

        Debug.Log(currentSessions.Count);

        sessionsDropDown.ClearOptions();
        sessionsDropDown.AddOptions(currentSessions);
        // GameObject.Find("GameChooser").GetComponent<GameList>().setGames(currentSessions); 
    }

    public void joinSession() {
        GameObject sessionId;
        if (GameObject.Find("sessionId") == null) {
            sessionId = (GameObject)Instantiate(SessionPrefab);
            sessionId.name = "sessionId";
        } else {
            sessionId = GameObject.Find("SessionId");
        }

        string sessIdStr = this.sessionsDropDown.options[sessionsDropDown.value].text;
        string urName = sessionId.GetComponent<SessionPrefabScript>().name;
        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();

        sessionId.GetComponent<SessionPrefabScript>().setSessionId(sessIdStr);
        LobbyCommands.joinSession(this, sessIdStr, token, urName);

        SceneManager.LoadScene("Lobby");
    }
}
