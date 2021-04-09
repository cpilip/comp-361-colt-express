using Coffee.UIEffects;
using GameUnitSpace;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLootAtLocationListener : UIEventListenable
{
    /*
     *   {
                eventName = action,
                position = (Position)args[0],
                index = int
                items = (List<GameItem>)args[1]

            };
     */
    public override void updateElement(string data)
    {
        JObject o = JObject.Parse(data);
        List<GameItem> g = o.SelectToken("items").ToObject<List<GameItem>>();
        Position p = o.SelectToken("position").ToObject<Position>();
        int index = o.SelectToken("index").ToObject<int>();

        GameObject lootPosition = null;
        //Stagecoach 
        if (index == -1)
        {
            lootPosition = GameUIManager.gameUIManagerInstance.getStageCoachLoot(p.isRoof());
        }
        else
        {
            lootPosition = GameUIManager.gameUIManagerInstance.getTrainCarLoot(index, p.isRoof());
        }

        foreach (Transform t in lootPosition.transform)
        {
            t.GetChild(0).gameObject.GetComponent<UIShiny>().enabled = true;
            t.GetChild(0).gameObject.GetComponent<Button>().enabled = true;
            t.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLoot);
        }
        
        Debug.Log("[UpdateLootAtLocationListener] Loot updated.");

    }
}
