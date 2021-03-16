using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTrainListener : UIEventListenable
{
    public GameObject diamondPrefab;
    public GameObject strongboxPrefab;
    public GameObject bagPrefab;
    public GameObject characterPrefab;
    public override void updateElement(string data)
    {
        Debug.Log("MAKING TRAIN");
        //PRE: data = TrainCar

        /* var definition = new {
                    eventName = action,
                    indexofCar = i,
                    i_items = n.getInside().getUnits_items(),
                    i_players = n.getInside().getUnits_players(),
                    r_items = n.getRoof().getUnits_items(),
                    r_players = n.getRoof().getUnits_players(),
                };*/

        JObject o = JObject.Parse(data);
        //List<GameItem> items = o.SelectToken("i_items").ToObject<<List<GameItem>>();
        int i = o.SelectToken("indexofCar").ToObject<int>();

        //GameUIManager.gameControllerInstance.createTrainCarObject(, );

        int carIndex = i + 5;

        //Create train car prefab

        GameObject roof = GameUIManager.gameControllerInstance.getTrainCarPosition(i);
        Debug.Log(roof);
        GameObject inter = GameUIManager.gameControllerInstance.getTrainCarPosition(carIndex);
        Debug.Log(inter);
        List<Character> r_P = o.SelectToken("r_players").ToObject<List<Character>>();
        List<ItemType> r_I = o.SelectToken("r_items").ToObject<List<ItemType>>();

        foreach (Character c in r_P)
        {
            GameUIManager.gameControllerInstance.createCharacterObject(c).transform.SetParent(roof.transform);
        }
        foreach (ItemType m in r_I)
        {
            if (m == ItemType.Purse)
            {
                Instantiate(bagPrefab).transform.SetParent(roof.transform);
            }
            if (m == ItemType.Strongbox)
            {
                Instantiate(strongboxPrefab).transform.SetParent(roof.transform);
            }
            if (m == ItemType.Ruby)
            {
                Instantiate(diamondPrefab).transform.SetParent(roof.transform);
            }
        }

        r_P = o.SelectToken("i_players").ToObject<List<Character>>();
        r_I = o.SelectToken("i_items").ToObject<List<ItemType>>();

        foreach (Character c in r_P)
        {
            GameUIManager.gameControllerInstance.createCharacterObject(c).transform.SetParent(inter.transform);
        }
        foreach (ItemType m in r_I)
        {
            if (m == ItemType.Purse)
            {
                Instantiate(bagPrefab).transform.SetParent(inter.transform);
            }
            if (m == ItemType.Strongbox)
            {
                Debug.Log("Making s");
                Instantiate(strongboxPrefab).transform.SetParent(inter.transform);
            }
            if (m == ItemType.Ruby)
            {
                Instantiate(diamondPrefab).transform.SetParent(inter.transform);
            }
        }
    }
}
