using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFirstPlayerListener : UIEventListenable
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

        GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c).transform.GetChild(5).gameObject.SetActive(true);

        Debug.Log("[UpdateFirstPlayerListener] Player: " + c);

        //If there is no previous first player, set the player to be the previous first player
        //Otherwise, reset the previous first player's start and update to the new first player
        if (previousPlayer.HasValue)
        {
            if (previousPlayer.Value == c)
            {
                previousPlayer = c;
            }
            else
            {
                GameUIManager.gameUIManagerInstance.getPlayerProfileObject(previousPlayer.Value).transform.GetChild(5).gameObject.SetActive(false);
                previousPlayer = c;
            }
            
        }
        else
        {
            previousPlayer = c;
        }
    }
}
