using System;
using UnityEngine;

[Serializable]
public class RegisterGameRequest
{
    public string location;
    public string maxSessionPlayers;
    public string minSessionPlayers;
    public string name;
    public string webSupport;

    public RegisterGameRequest(string location,
        string maxSessionPlayers,
        string minSessionPlayers,
        string name,
        string webSupport) 
    {
        this.location = location;
        this.maxSessionPlayers = maxSessionPlayers;
        this.minSessionPlayers = minSessionPlayers;
        this.name = name;
        this.webSupport = webSupport;
    }
}
