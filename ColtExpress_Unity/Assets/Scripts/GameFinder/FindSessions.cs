using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FindSessions : MonoBehaviour
{
    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    List<string> gameSessions;

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

        SessionsInformation responseParsed = JsonUtility.FromJson<SessionsInformation>(response);

        Debug.Log(responseParsed.sessions);

        List<string> currentSessions = new List<string>();
        foreach (string sessInfo in responseParsed.sessions.Keys) {
            Debug.Log(sessInfo);
            // currentSessions.Add(sessInfo);
        }

        // GameObject.Find("GameChooser").GetComponent<GameList>().setGames(currentSessions); 
    }
}
