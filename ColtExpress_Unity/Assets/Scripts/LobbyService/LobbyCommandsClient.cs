using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Text;


public class LobbyCommandsClient
{
    // default local host : 127.0.0.1:4242
    // Azure server IP : 13.68.140.249
    public const string connectionIP = "52.152.132.200:443"; // IP of Azure server

    private string response;

    public string getResponse() 
    {
        return response;
    }

    public void signIn(MonoBehaviour caller, string user, string pass) 
    {
        // string pass = password.GetComponent<TMP_InputField>().text;
        string url = string.Format("http://{0}/oauth/token?grant_type=password&username={1}&password={2}", connectionIP, user, pass);

        // WWWForm formData = new WWWForm();
        // formData.AddField("user_oauth_approval", "true");
        // formData.AddField("_csrf", "19beb2db-3807-4dd5-9f64-6c733462281b");
        // formData.AddField("authorize", "true");

        string formData = "nothing";

        caller.StartCoroutine(postRequest(url, formData, true));
    }

    public void debugOnline(MonoBehaviour caller)
    {
        string url = string.Format("http://{0}/api/online", connectionIP);
        caller.StartCoroutine(getRequest(url, false));
    }

    public void signOut(MonoBehaviour caller, string token)
    {
        // TODO Get current token
        string url = string.Format("http://{0}/oauth/active?access_token={1}", connectionIP, token);
        caller.StartCoroutine(deleteRequest(url, false));
    }

    public void getRole(MonoBehaviour caller, string token)
    {
        // TODO get current token
        string url = string.Format("http://{0}/oauth/role?access_token={1}", connectionIP, token);
        caller.StartCoroutine(getRequest(url, false));
    }

    public void getAllUsers(MonoBehaviour caller, string token)
    {
        // TODO get current token
        string url = string.Format("http://{0}/api/users?access_token={1}", connectionIP, token);
        caller.StartCoroutine(getRequest(url, true));
    }

    public void getUsername(MonoBehaviour caller, string token)
    {
        string url = string.Format("http://{0}/oauth/username?access_token={1}", connectionIP, token);    
        caller.StartCoroutine(getRequest(url, true));
    }

    public void getGameServices(MonoBehaviour caller)
    {
        string url = string.Format("http://{0}/api/gameservices", connectionIP);
        caller.StartCoroutine(getRequest(url, true));
    }

    public void getGameService(MonoBehaviour caller, string name)
    {
        string url = string.Format("http://{0}/api/gameservices/{1}", connectionIP, name);
        caller.StartCoroutine(getRequest(url, true));
    }

    public void createSession(MonoBehaviour caller, string token, string creator, string game, string savegame)
    {
        // string pass = password.GetComponent<TMP_InputField>().text;
        string url = string.Format("http://{0}/api/sessions?access_token={1}", connectionIP, UnityWebRequest.UnEscapeURL(token));
        Debug.Log(url);

        // WWWForm formData = new WWWForm();
        // formData.AddField("game", game);
        // formData.AddField("creator", creator);
        // formData.AddField("savegame", savegame);

        string formData = "{\"creator\":\"" + creator + "\", \"game\":\"" + game + "\", \"savegame\":\"" + savegame + "\"}";
        Debug.Log(formData);
        caller.StartCoroutine(postRequest(url, formData, true));
    }

    public void deleteSession(MonoBehaviour caller, string sessionID, string token)
    {
        string url = string.Format("http://{0}/api/sessions/{1}?access_token={2}", connectionIP, sessionID, token);
        caller.StartCoroutine(deleteRequest(url, true));
    }

    public void getSessionDetails(MonoBehaviour caller, string sessionID)
    {
        string url = string.Format("http://{0}/api/sessions/{1}", connectionIP, sessionID);
        caller.StartCoroutine(getRequest(url, true));
    }

    public void getSessions(MonoBehaviour caller)
    {
        string url = string.Format("http://{0}/api/sessions", connectionIP);
        caller.StartCoroutine(getRequest(url, true));
    }

    public void joinSession(MonoBehaviour caller, string sessionID, string name, string token)
    {
        string data = "";
        string url = string.Format("http://{0}/api/sessions/{1}/players/{2}?access_token={3}", connectionIP, sessionID, name, token);
        caller.StartCoroutine(putRequest(url, data, true, false));
    }

    public void launchSession(MonoBehaviour caller, string sessionID, string token)
    {
        // WWWForm form = new WWWForm();
        string form = "";
        string url = string.Format("http://{0}/api/sessions/{1}?access_token={2}", connectionIP, sessionID, token);
        caller.StartCoroutine(postRequest(url, form, true));
    }

    public void leaveSession(MonoBehaviour caller, string sessionID, string name, string token)
    {
        string url = string.Format("http://{0}/api/sessions/{1}/players/{2}?access_token={3}", connectionIP, sessionID, name, token);
        caller.StartCoroutine(deleteRequest(url, true));
    }

    // Methods for general requests

    private IEnumerator getRequest(string url, bool auth) 
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            if (auth)
            {
                // Define the header of the request for Security purposes
                string header = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                webRequest.SetRequestHeader("Authorization", header);
            }

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                // Debug.Log(":Received: " + webRequest.downloadHandler.text);
                response = webRequest.downloadHandler.text;
            }
        }
    }

    // Private methods that handle requests
    private IEnumerator deleteRequest(string url, bool auth)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
        {
            if (auth) 
            {
                // Define the header of the request for Security purposes
                string header = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                webRequest.SetRequestHeader("Authorization", header);
            }

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                // Debug.Log(":Received: " + webRequest.downloadHandler.text);
                response = webRequest.downloadHandler.text;
            }
        }
    }

    private IEnumerator postRequest(string url, string formData, bool auth)
    {
        
        // Configure the data portion of the post request 
        // using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        // {
        //     if (auth) 
        //     {
        //         // Define the header of the request for Security purposes
        //         string header = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
        //         webRequest.SetRequestHeader("Authorization", header);
        //     }
        //     webRequest.SetRequestHeader("Content-Type", "application/json");
            
        //     // Request and wait for the desired page.
        //     yield return webRequest.SendWebRequest();

        //     if (webRequest.isNetworkError)
        //     {
        //         Debug.Log(": Error: " + webRequest.error);
        //     }
        //     else
        //     {
        //         // Debug.Log(":Received: " + webRequest.downloadHandler.text);
        //         response = webRequest.downloadHandler.text;
        //     }
        // }

        var webRequest = new UnityWebRequest (url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(formData);
        webRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        if (auth) 
        {
            // Define the header of the request for Security purposes
            string header = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
            webRequest.SetRequestHeader("Authorization", header);
        }

        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
        {
            Debug.Log(": Error: " + webRequest.error);
        }
        else
        {
            // Debug.Log(":Received: " + webRequest.downloadHandler.text);
            response = webRequest.downloadHandler.text;
        }
    }

    private IEnumerator putRequest(string url, string data, bool auth, bool gsRequest)
    {
        // Configure the data portion of the post request 
        using (UnityWebRequest webRequest = UnityWebRequest.Put(url, data))
        {
            if (auth) 
            {
                // Define the header of the request for Security purposes
                string header = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                webRequest.SetRequestHeader("Authorization", header);
            }

            if (gsRequest)
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
            }

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                // Debug.Log(":Received: " + webRequest.downloadHandler.text);
                response = webRequest.downloadHandler.text;
            }
        }
    }

}   
