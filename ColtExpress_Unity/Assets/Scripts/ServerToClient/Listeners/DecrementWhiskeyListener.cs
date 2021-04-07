using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecrementWhiskeyListener : UIEventListenable
{

    public override void updateElement(string data)
    {
        /* PRE: data 
        *
            {
                eventName = action,
                player = (Character)args[0],
                whiskey = (WhiskeyKind)args[1]
            };
        */

        JObject o = JObject.Parse(data);
        Character c = o.SelectToken("player").ToObject<Character>();
        WhiskeyKind w = o.SelectToken("whiskey").ToObject<WhiskeyKind>();

        GameObject playerProfile = GameUIManager.gameUIManagerInstance.getPlayerProfileObject(c);

        string value = "";
        int num = 0;

        switch (w)
        {
            case WhiskeyKind.Unknown:
                //3 - Usables, 0 - Full/Unknown, 2 - Text
                value = playerProfile.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num--;

                playerProfile.transform.GetChild(3).GetChild(0).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num <= 0)
                {
                    playerProfile.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
                }

                break;
            case WhiskeyKind.Normal:
                //3 - Usables, 1 - Normal, 2 - Text
                value = playerProfile.transform.GetChild(3).GetChild(1).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num--;

                playerProfile.transform.GetChild(3).GetChild(1).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num <= 0)
                {
                    playerProfile.transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
                }

                break;
            case WhiskeyKind.Old:
                //3 - Usables, 2 - Old, 2 - Text
                value = playerProfile.transform.GetChild(3).GetChild(2).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num--;

                playerProfile.transform.GetChild(3).GetChild(2).GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num <= 0)
                {
                    playerProfile.transform.GetChild(3).GetChild(2).gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }

        Debug.Log("[DecrementWhiskeyListener] Decremented " + w + " for player " + c + ".");

    }
}
