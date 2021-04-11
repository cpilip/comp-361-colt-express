using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identification : MonoBehaviour
{
    public string currToken;
    public string refreshToken;
    public string username;

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
