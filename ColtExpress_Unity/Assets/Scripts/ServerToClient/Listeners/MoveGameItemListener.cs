using GameUnitSpace;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveGameItemListener : UIEventListenable
{

    public override void updateElement(string data)
    {
        /* PRE: data
        * 
            {
                    eventName = action,
                    gameItem = (GameItem) args[0],
                    position = (Position)args[1],
                    trainCarIndex = (int)args[2]
                };
        */
        JObject o = JObject.Parse(data);
        int i = o.SelectToken("trainCarIndex").ToObject<int>();
        Position p = o.SelectToken("position").ToObject<Position>();
        GameItem g = o.SelectToken("gameItem").ToObject<GameItem>();

        GameObject lootStrip = null;
        if (i == -1)
        {

            lootStrip = GameUIManager.gameUIManagerInstance.getStageCoachLoot(p.isRoof());
        }
        else
        {
            lootStrip = GameUIManager.gameUIManagerInstance.getTrainCarLoot(i, p.isRoof());
        }

        string value = "";
        int num = 0;

        int index = 0;
        switch (g.getType())
        {
            case ItemType.Purse:
                index = 3;
                break;
            case ItemType.Ruby:
                index = 4;
                break;
            case ItemType.Strongbox:
                index = 5;
                break;
            case ItemType.Whiskey:
                Whiskey w = o.SelectToken("gameItem").ToObject<Whiskey>();

                switch (w.getWhiskeyKind())
                {
                    case WhiskeyKind.Unknown:
                        index = 0;
                        break;
                    case WhiskeyKind.Old:
                        index = 1;
                        break;
                    case WhiskeyKind.Normal:
                        index = 2;
                        break;
                }
                break;
        }

        
        value = lootStrip.transform.GetChild(index).GetChild(1).GetComponent<TextMeshProUGUI>().text;
        num = Int32.Parse(value.Substring(1));
        num++;
        lootStrip.transform.GetChild(index).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

        Debug.Log("[MoveGameItemListener] Item " + g.getType() + " dropped.");

        if (num > 0)
        {
            lootStrip.transform.GetChild(index).gameObject.SetActive(true);
        }

    }
}
