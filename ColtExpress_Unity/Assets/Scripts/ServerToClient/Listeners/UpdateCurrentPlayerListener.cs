using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCurrentPlayerListener : UIEventListenable
{
    private static Character? previousPlayer = null;
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

        GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c).GetComponent<Image>().color = new Color(1.000f, 0.933f, 0.427f, 0.914f);

        Debug.Log("[UpdateCurrentPlayerListener] Player: " + c);

        //If there is no previous player, set the current player to the previous player
        //Otherwise, reset the previous player's highlighting and update to the new player
        if (previousPlayer.HasValue)
        {
            if (c != previousPlayer)
            {
                //If the current player is different from the previous player, reset the previous player's profile color 
                //Otherwise, they're the same player
                GameUIManager.gameUIManagerInstance.getPlayerProfileObject(previousPlayer.Value).GetComponent<Image>().color = new Color(1.000f, 1f, 1f, 1f);
            }
            previousPlayer = c;

        } else
        {
            previousPlayer = c;
        }


    }
}
