using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identification : MonoBehaviour
{
    public string currToken;
    public string refreshToken;
    public string username;

    private float initTime;

    private float time = 0.0f;
    private float interpolationPeriod = 1750.0f;

    void Update() {
        // Update all the names of the players currently in the lobby
        time += Time.deltaTime;
 
        if (time >= interpolationPeriod) {
            time = 0.0f;
            StartCoroutine(refreshWait(1.0f));
        }
    }

    private IEnumerator refreshWait(float time)
    {


        // Call lobby service to create session
        LobbyCommands.refreshToken(this, this.refreshToken);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);

        SignInResponse json = JsonUtility.FromJson<SignInResponse>(response);

        if (json.access_token == null){
            Debug.Log("error refreshing token");
        } else {
            Debug.Log("Refreshed Token! Expires in " + json.expires_in);
            this.setToken(UnityWebRequest.EscapeURL(json.access_token), UnityWebRequest.EscapeURL(json.refresh_token));
            this.setUsername(usernameField.GetComponent<TMP_InputField>().text);
            this.interpolationPeriod = json.expires_in;
        }
    }

    public void setRefreshTime(float time) {
        this.interpolationPeriod = time;
    }

    public void setToken(string token, string refToken)
    {
        this.currToken = token;
        this.refreshToken = refToken;
    }

    public string getToken()
    {
        return this.currToken;
    }

    public string getUsername()
    {
        return this.username;
    }

    public void setUsername(string name)
    {
        this.username = name;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
