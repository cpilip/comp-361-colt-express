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

    public void logIn() {
        LobbyCommands.signIn(this, usernameField.GetComponent<TMP_InputField>().text, passwordField.GetComponent<TMP_InputField>().text);
        Debug.Log(LobbyCommands.getResponse());
        GameObject id = (GameObject)Instantiate(IDPrefab);
        id.name = "ID";
        id.GetComponent<Identification>().setToken("FAKE_TOKEN", "FAKE_REFRESH_TOKEN");
    }
}
