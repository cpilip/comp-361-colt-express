using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWaitingForInputListener : UIEventListenable
{

    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = "updateWaitingForInput",
                currentPlayer = c
            };
        */
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("currentPlayer").ToObject<Character>();

        if (NamedClient.c == player)
        {
            //If SCHEMIN, unlock the turn menu
            if (GameUIManager.gameUIManagerInstance.gameStatus)
            {
                GameUIManager.gameUIManagerInstance.unlockTurnMenu();


                Debug.Log("[UpdateWaitingForInputListener] Turn menu unlocked for player " + player + ".");
            } else
            {
                //GameUIManager.gameUIManagerInstance.unlockTurnMenu();


                Debug.Log("[UpdateWaitingForInputListener] Board unlocked for player " + player + ".");
            }
        }
        
    }
}