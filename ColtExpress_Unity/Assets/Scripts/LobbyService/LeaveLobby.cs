using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LeaveLobby : MonoBehaviour
{
    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();


    public void leaveLobby() {
        GameObject sessionId = GameObject.Find("SessionId");
        bool creator = sessionId.GetComponent<SessionPrefabScript>().getCreator();
        if (creator) {
            StartCoroutine(waitDelete(1));
        } else {
            StartCoroutine(waitLeave(1));
        }
    }

    private IEnumerator waitDelete(float time) {
        
        if (GameObject.Find("sessionId") == null) {
            Debug.Log("we have a problem");
        }

        GameObject sessionId = GameObject.Find("SessionId");
        string id = sessionId.GetComponent<SessionPrefabScript>().getSessionId();

        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.deleteSession(this, id, token);
            
        // Call lobby service to delete session
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);
        SceneManager.LoadScene ("Play");

    }

    private IEnumerator waitLeave(float time) {
        
        if (GameObject.Find("sessionId") == null) {
            Debug.Log("we have a problem");
        }

        GameObject sessionId = GameObject.Find("SessionId");
        string id = sessionId.GetComponent<SessionPrefabScript>().getSessionId();
        string name = GameObject.Find("ID").GetComponent<Identification>().getUsername();

        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.leaveSession(this, id, name, token);
            
        // Call lobby service to delete session
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);
        SceneManager.LoadScene ("Play");

    }
}
