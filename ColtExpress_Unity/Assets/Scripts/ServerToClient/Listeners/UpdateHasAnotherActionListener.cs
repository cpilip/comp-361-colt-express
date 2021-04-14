using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHasAnotherActionListener : UIEventListenable
{

    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = "updateHasAnotherAction",
                currentPlayer = character,
                hasAnotherAction = bool,
                otherAction = string
            };
        */
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("currentPlayer").ToObject<Character>();
        bool hasAnotherAction = o.SelectToken("hasAnotherAction").ToObject<bool>();
        string otherAction = o.SelectToken("otherAction").ToObject<string>();

        if (NamedClient.c == player)
        {
            //If SCHEMIN, unlock the turn menu
            if (GameUIManager.gameUIManagerInstance.gameStatus == GameStatus.Schemin)
            {
                if (hasAnotherAction)
                {
                    GameUIManager.gameUIManagerInstance.toggleTurnMenu(true);

                    //If hasAnotherAction + "play", must be first call from useWhiskey()
                    if (otherAction == "play")
                    {
                        GameUIManager.gameUIManagerInstance.whiskeyWasUsed = true;
                    }

                    //If hasAnotherAction + "both" + whiskeyWasUsed, must be second call from useWhiskey()'s endOfTurn() 
                    if (otherAction == "both" && GameUIManager.gameUIManagerInstance.whiskeyWasUsed)
                    {
                        otherAction = "play";
                        GameUIManager.gameUIManagerInstance.whiskeyWasUsed = false;
                    }

                    GameUIManager.gameUIManagerInstance.toggleTurnMenuButtons(otherAction);
                    Debug.Log("[UpdateHasAnotherActionListener] SCHEMING, TRUE: Turn menu visible for player " + player + " and otherAction: " + otherAction + ".");
                }
                else
                {
                    GameUIManager.gameUIManagerInstance.toggleTurnMenu(false);
                    GameUIManager.gameUIManagerInstance.lockHand();
                    Debug.Log("[UpdateHasAnotherActionListener] SCHEMIN, FALSE: Turn menu hidden for player and hand locked for " + player + ".");
                }

            }
            else
            {
                Debug.Log("[UpdateHasAnotherActionListener] STEALIN.");
            }
        }
    }
}

