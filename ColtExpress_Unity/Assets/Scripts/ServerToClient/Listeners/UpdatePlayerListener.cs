using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        // PRE: data = Player packet
        Debug.Log("Making PLAYER!");
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("player").ToObject<Character>();

        /*
        var definition = new
        {
            eventName = action,
            player = n.getBandit(),
            h_ActionCards = n.getHand_actionCards(),
            h_BulletCards = n.getHand_bulletCards(),
            d_ActionCards = n.getDiscard_actionCards(),
            d_BulletCards = n.getDiscard_bulletCards()
        };*/

        GameUIManager.gameControllerInstance.createPlayerProfileObject(player);
    }
}
