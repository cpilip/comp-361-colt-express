﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindGameServices : MonoBehaviour
{
    public Dropdown dropdown;
    private LobbyCommandsClient LobbyCommands = new LobbyCommandsClient();
    private List<string> options = new List<string>();
    private bool updated;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(findGamesWait(1));
        dropdown.ClearOptions();
        if (options.Count == 0) {
            options.Add("None found");
        }
        dropdown.AddOptions(options);
    }

    public void refresh() {
        StartCoroutine(findGamesWait(1));
        dropdown.ClearOptions();
        if (options.Count == 0) {
            options.Add("None found");
        }
        dropdown.AddOptions(options);
    }

    private IEnumerator findGamesWait(float time)
    {
        LobbyCommands.getGameServices(this);
        yield return new WaitForSeconds(time);
        string response = LobbyCommands.getResponse();
        Debug.Log(response);
        response = response.Replace("[", "");
        response = response.Replace("]", "");
        response = response.Replace("\"", "");
        List<string> gameServicesNames = new List<string>(response.Split(','));
        foreach (string st in gameServicesNames)
        {
            Debug.Log(st);
        }
        options = gameServicesNames;
    }

}