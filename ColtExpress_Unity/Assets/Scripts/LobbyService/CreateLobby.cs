using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;



public class CreateLobby : MonoBehaviour
{
    public GameObject SavedGameText; 

    public Object SessionPrefab;

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    public void createLobby() {
        StartCoroutine(waitCreate(1));
    }

    public void leaveLobby() {
        GameObject sessionId = GameObject.Find("SessionId");
        boolean creator = sessionId.GetComponent<SessionPrefabScript>().getCreator();
        if (creator) {
            StartCoroutine(waitDelete(1));
        } else {
            StartCoroutine(waitLeave(1));
        }
    }

    private IEnumerator waitDelete(float time) {
        
        if (GameObject.Find("sessionId") != null) {
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

    }

    private IEnumerator waitLeave(float time) {
        
        if (GameObject.Find("sessionId") != null) {
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

    }

    private IEnumerator waitCreate(float time)
    {
        // Get necessary variables
        string name = SavedGameText.GetComponent<TMP_InputField>().text;
        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        string urName = GameObject.Find("ID").GetComponent<Identification>().getUsername();

        Debug.Log(token);

        // Call lobby service to create session
        LobbyCommands.createSession(this, token, urName, "ColtExpress", name);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);


        if (response == "")
        {
            SceneManager.LoadScene ("NewSession");
        }

        GameObject sessionId;
        if (GameObject.Find("sessionId") == null) {
            sessionId = (GameObject)Instantiate(SessionPrefab);
            sessionId.name = "sessionId";
        } else {
            sessionId = GameObject.Find("SessionId");
        }

        sessionId.GetComponent<SessionPrefabScript>().setSessionId(response);
        sessionId.GetComponent<SessionPrefabScript>().setUsername(urName);
        sessionId.GetComponent<SessionPrefabScript>().setCreator(true);
    }
    
}
