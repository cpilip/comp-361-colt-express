using HostageSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StealinPhaseManager : MonoBehaviour
{
    public GameObject timer;
    public GameObject hostageList;

    void Start()
    {
        
    }

    // Choose a Hostage
    public void chooseHostage()
    {
        //Unlock hostage menu
        GameUIManager.gameUIManagerInstance.toggleHostageMenu(true);
    }

    public void playerChoseHostage()
    {
        HostageChar hostage;

        Enum.TryParse(EventSystem.current.currentSelectedGameObject.name, out hostage);
        EventSystem.current.currentSelectedGameObject.SetActive(false);

        Debug.Log("[StealinPhaseManager - ChooseHostage] You chose [" + hostage + "].");

        var definition = new
        {
            eventName = "HostageMessage",
            chosenHostage = hostage
        };

        GameUIManager.gameUIManagerInstance.toggleHostageMenu(false);
        ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
    }

    public void playerChosePosition()
    {
        (bool, int) position = GameUIManager.gameUIManagerInstance.getTrainCarIndexByPosition(EventSystem.current.currentSelectedGameObject);

        var definition = new
        {
            eventName = "MoveMessage",
            index = position.Item2,
            inside = position.Item1
        };

        GameUIManager.gameUIManagerInstance.lockBoard();
        GameUIManager.gameUIManagerInstance.clearMovePositions();
        ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
    }
}
