using CardSpace;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ActionCantBePlayedListener : UIEventListenable
{
    public GameObject actionCantBePlayedPopup;
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
                currentPlayer = (Character)args[0]
            };
        */

        JObject o = JObject.Parse(data);
        ActionKind k = o.SelectToken("actionKind").ToObject<ActionKind>();

        string text = actionCantBePlayedPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        //You can't play the Punch Action Card anymore.
        //You can't play the 
        if (k == ActionKind.Punch)
        {
            text += "Punch Action Card anymore.";
            GameUIManager.gameUIManagerInstance.actionBlocked = (true, ActionKind.Punch);
        } else if (k == ActionKind.Ride)
        {
            text += "Ride Action Card anymore.";
            GameUIManager.gameUIManagerInstance.actionBlocked = (true, ActionKind.Ride);
        }

        if (GameUIManager.gameUIManagerInstance.actionBlocked.Item1 == true)
        {
            GameUIManager.gameUIManagerInstance.blockActionCards(GameUIManager.gameUIManagerInstance.actionBlocked.Item2);
        }

        actionCantBePlayedPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;

        Debug.Log("[ActionCantBePlayedListener] Informed the player that " + k.ToString() + "can no longer be played.");

        StartCoroutine("displayingActionCantBePlayedPopup");

    }

    private IEnumerator displayingActionCantBePlayedPopup()
    {
        DateTime start = DateTime.Now;
        TimeSpan overall = new TimeSpan(0, 0, 3);
        bool timeToDisappear = false;

        actionCantBePlayedPopup.SetActive(true);

        while (timeToDisappear == false)
        {
            if ((DateTime.Now) - start > overall)
            {
                actionCantBePlayedPopup.SetActive(false);
                timeToDisappear = true;
                yield return null;
            }
            else
            {
                yield break;
            }
        }
    }

}
