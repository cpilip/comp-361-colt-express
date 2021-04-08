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
                currentPlayer = character,
                waitingForInput = bool,
            };
        */
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("currentPlayer").ToObject<Character>();
        bool waitingForInput = o.SelectToken("waitingForInput").ToObject<bool>();

        if (NamedClient.c == player)
        {
            //If SCHEMIN, unlock the turn menu
            if (GameUIManager.gameUIManagerInstance.gameStatus == GameStatus.Schemin)
            {
                if (waitingForInput) { 
                    GameUIManager.gameUIManagerInstance.toggleTurnMenu(true);
                Debug.Log("[UpdateWaitingForInputListener] SCHEMING, TRUE: Turn menu visible for player " + player + ".");
                }
                else
                {
                    GameUIManager.gameUIManagerInstance.toggleTurnMenu(false);
                    GameUIManager.gameUIManagerInstance.lockHand();
                    Debug.Log("[UpdateWaitingForInputListener] SCHEMIN, FALSE: Turn menu hidden for player and hand locked for " + player + ".");
                }

            } else
            {
                Debug.Log("[UpdateWaitingForInputListener] STEALIN.");
            }
        }
        
    }
}