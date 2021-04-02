using CardSpace;
using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerHandListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
                {
                    eventName = "updatePlayerHand",
                    player = c,
                    cardsToAdd = l
                };
        */

        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("player").ToObject<Character>();

        if (player == NamedClient.c)
        {
            List<ActionKind> h_cards = o.SelectToken("cardsToAdd").ToObject<List<ActionKind>>();

            foreach (ActionKind a in h_cards)
            {
                GameUIManager.gameUIManagerInstance.createCardObject(player, a, true);
                foreach (Transform c in GameUIManager.gameUIManagerInstance.deck.transform)
                {
                    c.gameObject.SetActive(true);
                }
            }

            Debug.Log("[UpdatePlayerHandListener] Player hand updated for " + player + ".");
        }
    }

}



