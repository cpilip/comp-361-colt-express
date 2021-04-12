using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionPrefabScript : MonoBehaviour
{
    public string sessionID;
    public boolean isCreator;

    public setCreator(boolean b) {
        this.isCreator = b;
    }

    public boolean getCreator() {
        return this.isCreator;
    }

    public void setSessionId(string id)
    {
        this.sessionID = id;
    }

    public string getId()
    {
        return this.sessionID;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
