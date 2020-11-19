using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identification : MonoBehaviour
{
    private string currToken;
    private string refreshToken;

    public void setToken(string token, string refToken)
    {
        this.currToken = token;
        this.refreshToken = refToken;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
