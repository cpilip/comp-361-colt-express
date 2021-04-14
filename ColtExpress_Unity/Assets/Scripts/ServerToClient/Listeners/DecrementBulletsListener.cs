using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecrementBulletsListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
                player = (Character)args[0],
                numOfBullets = 6 - (int)args[1]
            };
        */

        JObject o = JObject.Parse(data);
        Character c = o.SelectToken("player").ToObject<Character>();
        int n = o.SelectToken("numOfBullets").ToObject<int>();

        GameObject playerProfile = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c);
        //2 - Inventory, 0 - Bullets, 1 - Text
        playerProfile.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", n);

        Debug.Log("[DecrementBulletListener] Decremented bullets for player " + c + ".");

    }
}
