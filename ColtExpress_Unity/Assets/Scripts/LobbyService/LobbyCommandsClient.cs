using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class LobbyCommandsClient
{
    private string response;

    public string getResponse() 
    {
        return response;
    }

    public void signIn(MonoBehaviour caller, string user, string pass) 
    {
        // string pass = password.GetComponent<TMP_InputField>().text;
        string url = string.Format("http://127.0.0.1:4242/oauth/token?grant_type=password&username={0}&password={1}", user, pass);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_oauth_approval=true&_csrf=19beb2db-3807-4dd5-9f64-6c733462281b&authorize=true"));

        caller.StartCoroutine(postRequest(url, formData, true));
    }

    public void debugOnline(MonoBehaviour caller)
    {
        string url = "http://127.0.0.1:4242/api/online";
        caller.StartCoroutine(getRequest(url, false));
    }

    public void signOut(MonoBehaviour caller, string token)
    {
        // TODO Get current token
        string url = string.Format("http://127.0.0.1:4242/oauth/active?access_token={0}", token);
        caller.StartCoroutine(deleteRequest(url, false));
    }

    public void getRole(MonoBehaviour caller, string token)
    {
        // TODO get current token
        string url = string.Format("http://127.0.0.1:4242/oauth/role?access_token={0}", token);
        caller.StartCoroutine(getRequest(url, false));
    }

    public void getAllUsers(MonoBehaviour caller, string token)
    {
        // TODO get current token
        string url = string.Format("http://127.0.0.1:4242/api/users?access_token={0}", token);
        caller.StartCoroutine(getRequest(url, true));
    }

    public void getUsername(MonoBehaviour caller, string token)
    {
        string url = string.Format("http://127.0.0.1:4242/oauth/username?access_token={0}", token);    
        caller.StartCoroutine(getRequest(url, true));
    }

    public void registerGameService(MonoBehaviour caller, string location, int maxPlayers, int minPlayers, string name, string webSupport, string token)
    {
        string requestData = string.Format("{\"location\": \"{0}}\",\"maxSessionPlayers\": \"{1}\",\"minSessionPlayers\": \"{2}\",\"name\": \"{3}\",\"webSupport\": \"{4}\"}", location, maxPlayers, minPlayers, name, webSupport);
        string url = string.Format("http://127.0.0.1:4242/api/gameservices/{0}}?access_token={1}", name, token);

        caller.StartCoroutine(putRequest(url, requestData, true));
    }

    public void unregisterGameService(MonoBehaviour caller, string name, string token) 
    {
        string url = string.Format("http://127.0.0.1:4242/api/gameservices/{0}?access_token={1}", name, token);

        caller.StartCoroutine(deleteRequest(url, true));
    }


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
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
                this.response = webRequest.downloadHandler.text;
            }
        }
    }

    // Private methods that handle requests
    private IEnumerator deleteRequest(string token, bool auth)
    {
        string url = string.Format("http://127.0.0.1:4242/oauth/active?access_token={0}", token);
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
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
                this.response = webRequest.downloadHandler.text;
            }
        }
    }

    private IEnumerator postRequest(string url, List<IMultipartFormSection> formData, bool auth)
    {
        // Configure the data portion of the post request 
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            if (auth) 
            {
                // Define the header of the request for Security purposes
                string header = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                webRequest.SetRequestHeader("Authorization", header);
            }
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
                this.response = webRequest.downloadHandler.text;
            }
        }
    }

    private IEnumerator putRequest(string url, string data, bool auth)
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

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
                this.response = webRequest.downloadHandler.text;
            }
        }
    }

}   
