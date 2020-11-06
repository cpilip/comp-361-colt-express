using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyCommandsClient : MonoBehaviour
{
    // public IEnumerator LogIn(string username, string password)
    // {
    //     WWWForm form = new WWWForm();
    // }

    public IEnumerator debugOnline() 
    {
        WWWForm form = new WWWForm();
        string url = "http://127.0.0.1:4242/api/online";

        using (var w = UnityWebRequest.Get(url))
        {
            yield return w.SendWebRequest();
            if (w.isNetworkError || w.isHttpError) {
                print(w.error);
            }
            else {
                print("Got a response");
            }
        }
    }
}   
