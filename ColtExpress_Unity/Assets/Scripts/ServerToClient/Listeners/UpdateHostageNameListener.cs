using GameUnitSpace;
using HostageSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateHostageNameListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
                player = (Character)args[0],
                hostage = (HostageChar)arg[1]
            };
        */

        JObject o = JObject.Parse(data);
        Character c = o.SelectToken("player").ToObject<Character>();
        HostageChar h = o.SelectToken("hostage").ToObject<HostageChar>();

        string displayHostageName = "";

        switch (h)
        {
            case (HostageChar.LadyPoodle):
                displayHostageName = "Lady's Poodle";
                break;
            case (HostageChar.Banker):
                displayHostageName = "Banker";
                break;
            case (HostageChar.Minister):
                displayHostageName = "Minister";
                break;
            case (HostageChar.Teacher):
                displayHostageName = "Teacher";
                break;
            case (HostageChar.Zealot):
                displayHostageName = "Zealot";
                break;
            case (HostageChar.OldLady):
                displayHostageName = "Old Lady";
                break;
            case (HostageChar.PokerPlayer):
                displayHostageName = "Poker Player";
                break;
            case (HostageChar.Photographer):
                displayHostageName = "Photographer";
                break;
        }

        GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = displayHostageName;

        Debug.Log("[UpdateHostageNameListener] Player: " + c.ToString() + " has " + h.ToString());

    }
}
