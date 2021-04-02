using CardSpace;
using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePlayerListener : UIEventListenable
{

    public override void updateElement(string data)
    {
        /* PRE: data
        *
            {
                eventName = "updatePlayer",
                player = Character c,
                h_ActionCards = List<ActionKind>
                h_BulletCards = int,
                d_ActionCards = List<ActionKind>,
                d_BulletCards = int
            };
        */
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("player").ToObject<Character>();

        Debug.Log("[UpdatePlayerListener] Player profile created for " + player + ".");
        GameUIManager.gameUIManagerInstance.createPlayerProfileObject(player);

        if (player == NamedClient.c)
        {
            Debug.Log("[UpdatePlayerListener] Player hand, discard pile, and remaining bullets initialized for " + player + ".");

            List<ActionKind> h_a = o.SelectToken("h_ActionCards").ToObject<List<ActionKind>>();

            foreach (ActionKind a in h_a)
            {
                GameUIManager.gameUIManagerInstance.createCardObject(player, a, true);    
            }

            List<ActionKind> d_a = o.SelectToken("d_ActionCards").ToObject<List<ActionKind>>();

            foreach (ActionKind a in d_a)
            {
                GameUIManager.gameUIManagerInstance.createCardObject(player, a, false);
            }

            //Player profile prefab:
            //  child 2 > Inventory
            //  child 0 > Whiskey group
            //  child 1 > Whiskey text

            //TODO: Bullet cards?

        }

    }
}
