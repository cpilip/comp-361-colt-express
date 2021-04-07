using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementLootListener : UIEventListenable
{

    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
                player = (Character)args[0],
                loot = (GameItem)args[1]
            };
        */

        JObject o = JObject.Parse(data);
        Character c = o.SelectToken("player").ToObject<Character>();
        GameItem l = o.SelectToken("loot").ToObject<GameItem>();

        GameObject playerProfile = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c);

        string value = "";
        int num = 0;

        switch (l.getType())
        {
            case ItemType.Strongbox:
                //2 - Inventory, 1 - Strongboxes, 1 - Text
                value = playerProfile.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num--;
                playerProfile.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", num);
                break;
            case ItemType.Ruby:
                //2 - Inventory, 2 - Rubies, 1 - Text
                value = playerProfile.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num--;
                playerProfile.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", num);
                break;
            case ItemType.Purse:
                //2 - Inventory, 3 - Bags, 1 - Text
                value = playerProfile.transform.GetChild(2).GetChild(3).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num--;
                playerProfile.transform.GetChild(2).GetChild(3).GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", num);
                break;
            case ItemType.Whiskey:
                ((Whiskey)l).getWhiskeyKind();

                var definition = new
                {
                    eventName = "incrementWhiskey",
                    player = c,
                    whiskey = ((Whiskey)l).getWhiskeyKind()
                };

                EventManager.TriggerEvent("incrementWhiskey", JsonConvert.SerializeObject(definition));

                break;
            default:
                break;
        }

        Debug.Log("[DecrementLootListener] Decremented " + l + "for player " + c + ".");

    }
}
