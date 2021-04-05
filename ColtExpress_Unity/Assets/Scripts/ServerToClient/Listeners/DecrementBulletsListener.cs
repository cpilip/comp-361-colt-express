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
                numOfBullets = 6 - (int)args[0]
            };
        */

        JObject o = JObject.Parse(data);
        int n = o.SelectToken("loot").ToObject<int>();

        GameObject playerProfile = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(NamedClient.c);
        //2 - Inventory, 0 - Bullets, 1 - Text
        playerProfile.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x0{0}", n);

        Debug.Log("[DecrementBulletListener] Decremented bullets.");

    }
}
