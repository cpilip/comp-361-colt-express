using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identification : MonoBehaviour
{
    public string currToken;
    public string refreshToken;

    public void setToken(string token, string refToken)
    {
        this.currToken = token;
        this.refreshToken = refToken;
    }

    public string getToken()
    {
        return this.currToken;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
