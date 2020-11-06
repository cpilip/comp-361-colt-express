using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public LobbyCommandsClient lobbyCommand;
    public static Main instance;

    // Start is called before the first frame update
    void Start()
    {
        lobbyCommand = GetComponent<LobbyCommandsClient>();
        instance = this;
    }
}
