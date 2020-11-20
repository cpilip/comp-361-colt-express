using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogIn : MonoBehaviour
{
    public GameObject usernameField;
    public GameObject passwordField;

    public Object IDPrefab;

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    private string responseSoFar;

    public void logIn() {
        StartCoroutine(wait(1));
    }

    IEnumerator wait(float time){
        LobbyCommands.signIn(this, usernameField.GetComponent<TMP_InputField>().text, passwordField.GetComponent<TMP_InputField>().text);
        GameObject id = (GameObject)Instantiate(IDPrefab);
        id.name = "ID";

        string response = LobbyCommands.getResponse();
        if (response == null) {
            yield return new WaitForSeconds(time);
            response = LobbyCommands.getResponse();
        }
        SignInResponse json = JsonUtility.FromJson<SignInResponse>(response);
        Debug.Log(json.access_token);
        Debug.Log(json.refresh_token);
        id.GetComponent<Identification>().setToken(json.access_token, json.refresh_token);
    }
}
