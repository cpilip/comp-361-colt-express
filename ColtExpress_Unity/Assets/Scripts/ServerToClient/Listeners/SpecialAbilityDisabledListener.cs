using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbilityDisabledListener : UIEventListenable
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

        GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c).transform.GetChild(6).gameObject.SetActive(true);

        if (NamedClient.c == c)
        {
            GameUIManager.gameUIManagerInstance.abilityDisabled = true;
        }

        Debug.Log("[Special] Player: " + c);

    }
}
