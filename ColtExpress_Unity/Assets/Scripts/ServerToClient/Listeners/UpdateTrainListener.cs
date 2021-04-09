using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTrainListener : UIEventListenable
{
    public GameObject rubyPrefab;
    public GameObject strongboxPrefab;
    public GameObject bagPrefab;
    public GameObject characterPrefab;

    private Vector3 scale = new Vector3(1f, 1f, 1f);
    public override void updateElement(string data)
    {
        /* PRE: data
        * 
            {
                    eventName = "updateTrain",
                    indexofCar = int,
                    i_items = item types (interior),
                    i_hasMarshal = true if yes,
                    i_hasShotgun = 
                    i_players = characters (interior),
                    r_items = item types (roof),
                    r_players = n.getRoof().getUnits_players(),
                    r_hasMarshal = true if yes
                    i_hasShotgun =
                };
        */

        JObject o = JObject.Parse(data);
        int i = o.SelectToken("indexofCar").ToObject<int>();

        //This line makes sure the train cars are correctly ordered
        GameUIManager.gameUIManagerInstance.initializeTrainCar(i);

        //Retrieve the roof and interior for this train car
        GameObject trainCarRoof = GameUIManager.gameUIManagerInstance.getTrainCarPosition(i, true);
        GameObject trainCarInterior = GameUIManager.gameUIManagerInstance.getTrainCarPosition(i, false);

        List<Character> r_P = o.SelectToken("r_players").ToObject<List<Character>>();
        List<ItemType> r_I = o.SelectToken("r_items").ToObject<List<ItemType>>();

        bool r_m = o.SelectToken("r_hasMarshal").ToObject<bool>();
        bool r_s = o.SelectToken("r_hasShotgun").ToObject<bool>();

        //Roof initialization
        foreach (Character c in r_P)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.createCharacterObject(c);
            character.transform.SetParent(trainCarRoof.transform);
            character.transform.localScale = scale;
        }
        foreach (ItemType m in r_I)
        {
            GameObject item = null;
            if (m == ItemType.Purse)
            {
                item = Instantiate(bagPrefab);
            }
            if (m == ItemType.Strongbox)
            {
                item = Instantiate(strongboxPrefab);
            }
            if (m == ItemType.Ruby)
            {
                item = Instantiate(rubyPrefab);
            }
            item.transform.SetParent(trainCarRoof.transform);
            item.transform.localScale = scale;
        }

        if (r_m)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.createCharacterObject(GameUnitSpace.Character.Marshal);
            character.transform.SetParent(trainCarRoof.transform);
            character.transform.localScale = scale;
        }

        if (r_s)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.getCharacterObject(GameUnitSpace.Character.Shotgun);
            character.transform.SetParent(trainCarRoof.transform);
            character.transform.localScale = scale;
        }

        r_P = o.SelectToken("i_players").ToObject<List<Character>>();
        r_I = o.SelectToken("i_items").ToObject<List<ItemType>>();
        
        r_m = o.SelectToken("i_hasMarshal").ToObject<bool>();
        r_s = o.SelectToken("i_hasShotgun").ToObject<bool>();

        //Interior initialization
        foreach (Character c in r_P)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.createCharacterObject(c);
            character.transform.SetParent(trainCarInterior.transform);
            character.transform.localScale = scale;
        }
        foreach (ItemType m in r_I)
        {
            GameObject item = null;
            if (m == ItemType.Purse)
            {
                item = Instantiate(bagPrefab);
            }
            if (m == ItemType.Strongbox)
            {
                item = Instantiate(strongboxPrefab);
            }
            if (m == ItemType.Ruby)
            {
                item = Instantiate(rubyPrefab);
            }
            item.transform.SetParent(trainCarInterior.transform);
            item.transform.localScale = scale;
        }

        if (r_m)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.createCharacterObject(GameUnitSpace.Character.Marshal);
            character.transform.SetParent(trainCarInterior.transform);
            character.transform.localScale = scale;
        }

        if (r_s)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.getCharacterObject(GameUnitSpace.Character.Shotgun);
            character.transform.SetParent(trainCarInterior.transform);
            character.transform.localScale = scale;
        }

        Debug.Log("[UpdateTrainListener] Train car " + i + " initialized.");
    }
}
