using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;


public class LogIn : MonoBehaviour
{
    public GameObject usernameField;
    public GameObject passwordField;
    public GameObject messageField;

    public Object IDPrefab;

    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();

    private string responseSoFar;

    public void logIn() {
        StartCoroutine(wait(1));
    }

    private IEnumerator wait(float time) {
        LobbyCommands.signIn(this, usernameField.GetComponent<TMP_InputField>().text, passwordField.GetComponent<TMP_InputField>().text);

        GameObject id;
        if (GameObject.Find("ID") == null) {
            id = (GameObject)Instantiate(IDPrefab);
            id.name = "ID";
        } else {
            id = GameObject.Find("ID");
        }

        string response = LobbyCommands.getResponse();
        if (response == null) {
            yield return new WaitForSeconds(time);
            response = LobbyCommands.getResponse();
        }

        Debug.Log("Resp : "+ response);
        SignInResponse json = JsonUtility.FromJson<SignInResponse>(response);
        
        Text mText = messageField.GetComponent<Text>(); 

        if (json.access_token == null){
            Debug.Log("Sign In failed, try again");
            mText.text = "Sign in failed, try again";
        } else {
            Debug.Log(json.access_token);
            Debug.Log(json.refresh_token);

            Debug.Log("Expires in " + json.expires_in);

            id.GetComponent<Identification>().setRefreshTime(json.expires_in);
            id.GetComponent<Identification>().setToken(UnityWebRequest.EscapeURL(json.access_token), UnityWebRequest.EscapeURL(json.refresh_token));
            id.GetComponent<Identification>().setUsername(usernameField.GetComponent<TMP_InputField>().text);
            mText.text = "Sign in successful";
        }
        
    }
}
