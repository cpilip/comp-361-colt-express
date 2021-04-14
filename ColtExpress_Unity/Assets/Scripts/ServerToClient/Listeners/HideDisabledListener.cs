using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDisabledListener : UIEventListenable
{
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
        Character c = o.SelectToken("currentPlayer").ToObject<Character>();

        GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c).transform.GetChild(7).gameObject.SetActive(true);

        if (NamedClient.c == c)
        {
            GameUIManager.gameUIManagerInstance.photographerHideDisabled = true;

            if (c == Character.Ghost)
            {
                GameUIManager.gameUIManagerInstance.abilityDisabled = true;
            }
        }

        Debug.Log("[HideDisabledListener] Player: " + c);

    }
}
