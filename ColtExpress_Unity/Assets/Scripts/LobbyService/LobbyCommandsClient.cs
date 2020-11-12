using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class LobbyCommandsClient : MonoBehaviour
{

    public TMP_InputField username;
    public TMP_InputField password;

    public void signIn() 
    {
        string user = username.GetComponent<TMP_InputField>().text;
        string pass = password.GetComponent<TMP_InputField>().text;
        StartCoroutine(signInReq(user, pass));
    }

    private IEnumerator signInReq(string user, string pass)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user_oauth_approval=true&_csrf=19beb2db-3807-4dd5-9f64-6c733462281b&authorize=true"));

        string header = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));

        string url = string.Format("http://127.0.0.1:4242/oauth/token?grant_type=password&username={0}&password={1}", user, pass);
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            webRequest.SetRequestHeader("Authorization", header);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":Received: " + webRequest.downloadHandler.text);
            }
        }
    }

    public void debugOnline() 
    {
        StartCoroutine(debugReq());
    }

    private IEnumerator debugReq() 
    {
        string url = "http://127.0.0.1:4242/api/online";
        //string url = "www.google.com";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
}   
