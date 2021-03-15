using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class CreateLobby : MonoBehaviour
{
    public GameObject GameSessionNameText; 
    public GameObject GameUserNameText;
    public Dropdown gameDropDown;

    public Object SessionPrefab;

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    public void createLobby() {
        StartCoroutine(wait(1));
    }

    private IEnumerator wait(float time)
    {
        string name = GameSessionNameText.GetComponent<TMP_InputField>().text;
        string urName = GameUserNameText.GetComponent<TMP_InputField>().text;
        string token = GameObject.Find("ID").GetComponent<Identification>().getToken();
        LobbyCommands.createSession(this, token, urName, gameDropDown.options[gameDropDown.value].text, "");
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
        sessionId.GetComponent<SessionPrefabScript>().name = urName;

    }
}
