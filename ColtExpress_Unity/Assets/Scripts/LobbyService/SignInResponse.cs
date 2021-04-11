using UnityEngine;
using System;

[Serializable]
public class SignInResponse
{
    public string access_token;
    public string token_type;
    public string refresh_token;
    public int expires_in;
    public string scope;
}