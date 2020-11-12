using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class LobbyCommandsClient
{
    private string response;

    public void signIn() 
    {
        string user = username.GetComponent<TMP_InputField>().text;
        string pass = password.GetComponent<TMP_InputField>().text;
        string url = string.Format("http://127.0.0.1:4242/oauth/token?grant_type=password&username={0}&password={1}", user, pass);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_oauth_approval=true&_csrf=19beb2db-3807-4dd5-9f64-6c733462281b&authorize=true"));

        StartCoroutine(postRequest(url, formData, true));
    }

    public void debugOnline()
    {
        string url = "http://127.0.0.1:4242/api/online";
        StartCoroutine(getRequest(url, false));
    }

    public void signOut()
    {
        // TODO Get current token
        string token = "INVALID_TOKEN";
        string url = string.format("http://127.0.0.1:4242/oauth/active?access_token={0}", token);
        StartCoroutine(deleteRequest(currToken));
    }

    public void getRole()
    {
        // TODO get current token
        string token = "INVALID_TOKEN";
        string url = string.format("http://127.0.0.1:4242/oauth/role?access_token={0}", token);
        StartCoroutine(getRequest(url, false));
    }

    public void getAllUsers()
    {
        // TODO get current token
        string token = "INVALID_TOKEN";
        string url = string.format("http://127.0.0.1:4242/api/users?access_token={0}", token);
        StartCoroutine(getRequest(url, true));
    }

    public void getUsername()
    {
        string token = "INVALID_TOKEN";
        string url = string.format("http://127.0.0.1:4242/oauth/username?access_token={0}", token);    
        StartCoroutine(getRequest(url, true));
    }

    public void registerGameService()
    {
        // Get all the necessary information
        string location = "INVALID_LOCATION";
        int maxPlayers = 5;
        int minPlayers = 3;
        string name = "INVALID_NAME";
        string webSupport = true;
        string requestData = string.Format("{\"location\": \"{0}}\",\"maxSessionPlayers\": \"{1}\",\"minSessionPlayers\": \"{2}\",\"name\": \"{3}\",\"webSupport\": \"{4}\"}", location, maxPlayers, minPLayers, name, webSupport);
        
        string token = "INVALID_TOKEN";
        string url = string.Format("http://127.0.0.1:4242/api/gameservices/{0}}?access_token={1}", name, token);

        StartCoroutine(putRequest(url, requestData, true));
    }

    public void unregisterGameService() 
    {
        string name = "INVALID_NAME";
        string token = "INVALID_TOKEN";
        string url = string.Format("http://127.0.0.1:4242/api/gameservices/{0}?access_token={1}", name, token);

        StartCoroutine(deleteRequest(url, true));
    }


    private IEnumerator getRequest(string url, bool auth) 
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url))
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
        string url = string.format("http://127.0.0.1:4242/oauth/active?access_token={0}", token);
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
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, data))
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
