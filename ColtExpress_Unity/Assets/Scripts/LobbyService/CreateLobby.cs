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

    private IEnumerator waitCreate(float time)
    {
        // Get necessary variables
        string name = SavedGameText.GetComponent<TMP_InputField>().text;
        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        string urName = GameObject.Find("ID").GetComponent<Identification>().getUsername();

        Debug.Log(token);

        // Call lobby service to create session
        LobbyCommands.createSession(this, token, urName, "ColtExpress2", name);
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
            sessionId = GameObject.Find("sessionId");
        }

        sessionId.GetComponent<SessionPrefabScript>().setSessionId(response);
        sessionId.GetComponent<SessionPrefabScript>().setCreator(true);

        SceneManager.LoadScene ("Lobby");
    }
    
}
