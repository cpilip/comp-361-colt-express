using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHorsesListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /*{
            eventName = action,
                horseCount = ((List<AttackPosition>)args[0]).Count,
                players = l
            };
        */
        JObject o = JObject.Parse(data);
        List<Character> players = o.SelectToken("players").ToObject<List<Character>>();
        int horseCount = o.SelectToken("horseCount").ToObject<int>();

        foreach (Character c in players)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.createCharacterObject(c);
        }

        GameUIManager.gameUIManagerInstance.initializeHorses(players);

        Debug.Log("[UpdateHorsesListener] Horses initialized.");
    }
}
