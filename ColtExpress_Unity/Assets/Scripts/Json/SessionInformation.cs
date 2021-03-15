using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionsInformation
{
    public Dictionary<string, SessionInformation> sessions {get; set;}
}

public class SessionInformation
{
    public string creator {get; set;}
    public GameParameter gameParameters {get; set;}
    public bool launched {get; set;}
    public List<string> players {get; set;}
    public Dictionary<int, string> playerLocations {get; set;}
    public string savegameid;
}

public class GameParameter {
    public string location {get; set;}
    public int maxSessionPlayers {get; set;}
    public int minSessionPlayers {get; set;}
    public string name {get; set;}
    public string webSupport {get; set;}
}
