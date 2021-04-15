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
                    player = n.getBandit(),
                    numOfBullets = 6 - n.getNumOfBulletsShot()
            };
        */
        JObject o = JObject.Parse(data);
        Character player = o.SelectToken("player").ToObject<Character>();
        int numOfBullets = o.SelectToken("numOfBullets").ToObject<int>();

        string username = o.SelectToken("username").ToObject<string>();

        Debug.Log("[UpdatePlayerListener] Player profile created for " + player + ".");
        
        //Create profile
        GameUIManager.gameUIManagerInstance.createPlayerProfileObject(player, username);

        //Update profile with correct number of bullets (2 - Inventory, 0 - Bullets, 1 - Text)
        GameUIManager.gameUIManagerInstance.getPlayerProfileObject(player).transform.GetChild(2).GetChild(0).GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "x" + numOfBullets;

    }
}
