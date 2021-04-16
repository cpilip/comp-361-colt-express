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


    public void leaveLobby(bool delay) {
        GameObject sessionId = GameObject.Find("sessionId");
        bool creator = sessionId.GetComponent<SessionPrefabScript>().getCreator();
        if (creator) {
            StartCoroutine(waitDelete(1, delay));
        } else {
            StartCoroutine(waitLeave(1, delay));
        }
    }

    private IEnumerator waitDelete(float time, bool delay) {
        
        if (GameObject.Find("sessionId") == null) {
            Debug.Log("we have a problem");
        }

        // If we need to wait a delay, wait 15 seconds
        if (delay) {
            Debug.Log("Waiting before deletion");
            yield return new WaitForSeconds(15);
        }

        GameObject sessionId = GameObject.Find("sessionId");
        string id = sessionId.GetComponent<SessionPrefabScript>().getSessionId();

        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.deleteSession(this, id, token);
            
        // Call lobby service to delete session
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().setCreator(false);
        GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().setSessionId("");

        if (!delay) {
            SceneManager.LoadScene ("Play");
        }
    }

    private IEnumerator waitLeave(float time, bool delay) {
        
        if (GameObject.Find("sessionId") == null) {
            Debug.Log("we have a problem");
        }

        // If we need to wait a delay, wait 5 seconds
        if (delay) {
            Debug.Log("Waiting before leaving session");
            yield return new WaitForSeconds(5);
        }

        GameObject sessionId = GameObject.Find("sessionId");
        string id = sessionId.GetComponent<SessionPrefabScript>().getSessionId();
        string name = GameObject.Find("ID").GetComponent<Identification>().getUsername();

        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.leaveSession(this, id, name, token);
            
        // Call lobby service to delete session
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().setCreator(false);
        GameObject.Find("sessionId").GetComponent<SessionPrefabScript>().setSessionId("");

        if (!delay) {
            SceneManager.LoadScene ("Play");
        }

    }
}
