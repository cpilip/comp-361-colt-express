﻿using Coffee.UIEffects;
using GameUnitSpace;
using HostageSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StealinPhaseManager : MonoBehaviour
{
    public GameObject timer;
    public GameObject hostageList;

    private Character punchTarget;
    private ItemType punchLoot;
    private WhiskeyStatus whiskeyStatus;
    private WhiskeyKind whiskeyKind;

    private bool notInPunch = true;

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
        if (notInPunch)
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

        } else
        {
            notInPunch = true;
            (bool, int) position = GameUIManager.gameUIManagerInstance.getTrainCarIndexByPosition(EventSystem.current.currentSelectedGameObject);

            if (punchLoot == ItemType.Whiskey)
            {
                var definition = new
                {
                    eventName = "PunchMessage",
                    target = punchTarget,
                    item = punchLoot,
                    whiskeyStatus = whiskeyStatus,
                    whiskeyKind = whiskeyKind,
                    index = position.Item2,
                    inside = position.Item1
                };

                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
            }
            else
            {
                var definition = new
                {
                    eventName = "PunchMessage",
                    target = punchTarget,
                    item = punchLoot,
                    index = position.Item2,
                    inside = position.Item1
                };

                ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
            }
            
        }
    }

    public void playerChoseTarget()
    {
        Character target = GameUIManager.gameUIManagerInstance.getCharacterByPlayerProfile(EventSystem.current.currentSelectedGameObject.transform.parent.gameObject);

        if (target != Character.Marshal)
        {
            var definition = new
            {
                eventName = "ShootMessage",
                target = target
            };

            GameUIManager.gameUIManagerInstance.lockSidebar();
            GameUIManager.gameUIManagerInstance.clearShootTargets();
            ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
        }
    }

    public void playerChoseTargetPunch()
    {
        notInPunch = false;
        Character target = Character.Marshal;
        if (GameUIManager.gameUIManagerInstance.getShotgunByShotgunButton(EventSystem.current.currentSelectedGameObject.transform.parent.gameObject))
        {
            //Shotgun punched
            var definition = new
            {
                eventName = "PunchMessage",
                isShotgun = true
            };

            GameUIManager.gameUIManagerInstance.togglePunchShotgunButton(false);
            GameUIManager.gameUIManagerInstance.lockSidebar();
            GameUIManager.gameUIManagerInstance.clearPunchTargets();
            ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);

        } else
        {
            //Character punched
            target = GameUIManager.gameUIManagerInstance.getCharacterByPlayerProfile(EventSystem.current.currentSelectedGameObject.transform.parent.gameObject);
            punchTarget = target;

            GameObject targetprofile = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(target);

            if (targetprofile.transform.GetChild(3).GetChild(0).gameObject.activeSelf)
            {
                targetprofile.transform.GetChild(3).GetChild(0).gameObject.GetComponent<OnWhiskeyUsed>().allowedForPunch = true;
                targetprofile.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = true;
            }

            if (targetprofile.transform.GetChild(3).GetChild(1).gameObject.activeSelf)
            {
                targetprofile.transform.GetChild(3).GetChild(1).gameObject.GetComponent<OnWhiskeyUsed>().allowedForPunch = true;
                targetprofile.transform.GetChild(3).GetChild(1).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = true;
            }

            if (targetprofile.transform.GetChild(3).GetChild(2).gameObject.activeSelf)
            {
                targetprofile.transform.GetChild(3).GetChild(2).gameObject.GetComponent<OnWhiskeyUsed>().allowedForPunch = true;
                targetprofile.transform.GetChild(3).GetChild(2).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = true;
            }

            //Strongbox
            string value = targetprofile.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
            int num = Int32.Parse(value.Substring(1));

            if (num > 0)
            {
                targetprofile.transform.GetChild(2).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = true;
                targetprofile.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Button>().enabled = true;
                targetprofile.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLootPunch);
            }

            //Rubies
            value = targetprofile.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
            num = Int32.Parse(value.Substring(1));

            if (num > 0)
            {
                targetprofile.transform.GetChild(2).GetChild(2).gameObject.GetComponent<UIShiny>().enabled = true;
                targetprofile.transform.GetChild(2).GetChild(2).gameObject.GetComponent<Button>().enabled = true;
                targetprofile.transform.GetChild(2).GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLootPunch);
            }

            //Purse
            value = targetprofile.transform.GetChild(2).GetChild(3).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
            num = Int32.Parse(value.Substring(1));

            if (num > 0)
            {
                targetprofile.transform.GetChild(2).GetChild(3).gameObject.GetComponent<UIShiny>().enabled = true;
                targetprofile.transform.GetChild(2).GetChild(3).gameObject.GetComponent<Button>().enabled = true;
                targetprofile.transform.GetChild(2).GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLootPunch);
            }

            GameUIManager.gameUIManagerInstance.clearPunchTargets();
            GameUIManager.gameUIManagerInstance.togglePunchShotgunButton(false);
        }

    }

    public void playerChoseLootPunch()
    {
        GameObject caller = EventSystem.current.currentSelectedGameObject;
        GameObject punchTargetProfile = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(punchTarget);

        switch (caller.name)
        {
            case "Full Whiskey":
                punchLoot = ItemType.Whiskey;
                whiskeyKind = WhiskeyKind.Unknown;
                whiskeyStatus = WhiskeyStatus.Full;
                break;
            case "Old Whiskey":
                punchLoot = ItemType.Whiskey;
                whiskeyKind = WhiskeyKind.Old;
                whiskeyStatus = WhiskeyStatus.Half;
                break;
            case "Normal Whiskey":
                punchLoot = ItemType.Whiskey;
                whiskeyKind = WhiskeyKind.Normal;
                whiskeyStatus = WhiskeyStatus.Half;
                break;
            default:
                //Caller must be from inventory
                caller = caller.transform.parent.gameObject;
                break;

        }

        switch (caller.name)
        {
            case "Strongboxes":
                punchLoot = ItemType.Strongbox;
                break;
            case "Rubies":
                punchLoot = ItemType.Ruby;
                break;
            case "Bags":
                punchLoot = ItemType.Purse;
                break;
        }

        punchTargetProfile.transform.GetChild(3).GetChild(0).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = false;
        punchTargetProfile.transform.GetChild(3).GetChild(1).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = false;
        punchTargetProfile.transform.GetChild(3).GetChild(2).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = false;

        punchTargetProfile.transform.GetChild(2).GetChild(1).gameObject.GetComponent<UIShiny>().enabled = false;
        punchTargetProfile.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Button>().enabled = false;
        punchTargetProfile.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLootPunch);

        punchTargetProfile.transform.GetChild(2).GetChild(2).gameObject.GetComponent<UIShiny>().enabled = false;
        punchTargetProfile.transform.GetChild(2).GetChild(2).gameObject.GetComponent<Button>().enabled = false;
        punchTargetProfile.transform.GetChild(2).GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLootPunch);

        punchTargetProfile.transform.GetChild(2).GetChild(3).gameObject.GetComponent<UIShiny>().enabled = false;
        punchTargetProfile.transform.GetChild(2).GetChild(3).gameObject.GetComponent<Button>().enabled = false;
        punchTargetProfile.transform.GetChild(2).GetChild(3).gameObject.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLootPunch);
       
        GameUIManager.gameUIManagerInstance.lockSidebar();

        var definition = new {
            eventName = "PunchPositionsRequestMessage",
            target = punchTarget
        };

        ClientCommunicationAPI.CommunicationAPI.sendMessageToServer(definition);
    }
}
