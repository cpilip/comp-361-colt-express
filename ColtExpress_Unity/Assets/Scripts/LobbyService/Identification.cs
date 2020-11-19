using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identification
{
    private string currToken;
    private string refreshToken;

    public void setToken(string token, string refToken)
    {
        this.currToken = token;
        this.refreshToken = refToken;
    }

}
