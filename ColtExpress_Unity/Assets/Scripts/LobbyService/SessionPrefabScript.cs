using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionPrefabScript : MonoBehaviour
{
    public string sessionID;
    public bool isCreator;

    public void setCreator(bool b) {
        this.isCreator = b;
    }

    public bool getCreator() {
        return this.isCreator;
    }

    public void setSessionId(string id)
    {
        this.sessionID = id;
    }

    public string getSessionId()
    {
        return this.sessionID;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
