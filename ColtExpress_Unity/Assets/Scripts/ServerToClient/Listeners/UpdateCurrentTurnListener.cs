using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCurrentTurnListener : UIEventListenable
{
    private static int previousTurn = 0;
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = "updateCurrentTurn",
                currentTurn = i
            };
        */

        JObject o = JObject.Parse(data);
        int t = o.SelectToken("currentTurn").ToObject<int>();

        this.transform.GetChild(t).GetComponent<Image>().color = new Color(1.000f, 0.933f, 0.427f, 0.914f);

        GameUIManager.gameUIManagerInstance.isNormalTurn = (this.transform.GetChild(t).gameObject.name == "Standard") ? true : false;
        GameUIManager.gameUIManagerInstance.isTunnelTurn = (this.transform.GetChild(t).gameObject.name == "Tunnel") ? true : false;
        //GameUIManager.gameUIManagerInstance.hasAnotherAction = (this.transform.GetChild(t).gameObject.name == "SpeedingUp") ? true : false;

        Debug.Log("[UpdateCurrentTurnListener] Turn: " + t);

        t = (t == 0) ? (this.transform.childCount - 1) : t - 1;
        previousTurn = t;

        this.transform.GetChild(t).GetComponent<Image>().color = new Color(1.000f, 1f, 1f, 1f);

        

    }
}
