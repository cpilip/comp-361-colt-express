using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyCommandsClient : MonoBehaviour
{

    public void debugOnline() 
    {
        StartCoroutine(OnResponse());
    }

    private IEnumerator OnResponse() 
    {
        //string url = "http://127.0.0.1:4242/api/online";
        string url = "www.google.com";
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
