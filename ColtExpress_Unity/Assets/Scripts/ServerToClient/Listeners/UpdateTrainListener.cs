using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        //Retrieve the roof loot and interior loot for this train car
        GameObject trainCarRoofLoot = GameUIManager.gameUIManagerInstance.getTrainCarLoot(i, true);
        GameObject trainCarInteriorLoot = GameUIManager.gameUIManagerInstance.getTrainCarLoot(i, false);

        List<Character> r_P = o.SelectToken("r_players").ToObject<List<Character>>();
        List<ItemType> r_I = o.SelectToken("r_items").ToObject<List<ItemType>>();

        bool r_m = o.SelectToken("r_hasMarshal").ToObject<bool>();
        bool r_s = o.SelectToken("r_hasShotgun").ToObject<bool>();

        string value = "";
        int num = 0;

        //Roof initialization
        foreach (Character c in r_P)
        {
            GameObject character = GameUIManager.gameUIManagerInstance.createCharacterObject(c);
            character.transform.SetParent(trainCarRoof.transform);
            character.transform.localScale = scale;
        }
        foreach (ItemType m in r_I)
        {
            if (m == ItemType.Purse)
            {
                value = trainCarRoofLoot.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarRoofLoot.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarRoofLoot.transform.GetChild(3).gameObject.SetActive(true);
                }
            }
            if (m == ItemType.Strongbox)
            {
                value = trainCarRoofLoot.transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarRoofLoot.transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarRoofLoot.transform.GetChild(5).gameObject.SetActive(true);
                }
            }
            if (m == ItemType.Ruby)
            {
                value = trainCarRoofLoot.transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarRoofLoot.transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarRoofLoot.transform.GetChild(4).gameObject.SetActive(true);
                }
            }
            if (m == ItemType.Whiskey)
            {
                value = trainCarRoofLoot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarRoofLoot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarRoofLoot.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
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
            if (m == ItemType.Purse)
            {
                value = trainCarInteriorLoot.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarInteriorLoot.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarInteriorLoot.transform.GetChild(3).gameObject.SetActive(true);
                }
            }
            if (m == ItemType.Strongbox)
            {
                value = trainCarInteriorLoot.transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarInteriorLoot.transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarInteriorLoot.transform.GetChild(5).gameObject.SetActive(true);
                }
            }
            if (m == ItemType.Ruby)
            {
                value = trainCarInteriorLoot.transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarInteriorLoot.transform.GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarInteriorLoot.transform.GetChild(4).gameObject.SetActive(true);
                }
            }
            if (m == ItemType.Whiskey)
            {
                value = trainCarInteriorLoot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text;
                num = Int32.Parse(value.Substring(1));
                num++;
                trainCarInteriorLoot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("x{0}", num);

                if (num > 0)
                {
                    trainCarInteriorLoot.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
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
