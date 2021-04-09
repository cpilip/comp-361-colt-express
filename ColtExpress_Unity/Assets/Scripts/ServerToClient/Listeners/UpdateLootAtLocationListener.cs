using Coffee.UIEffects;
using GameUnitSpace;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System;
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
        bool refresh = o.SelectToken("refresh").ToObject<bool>();

        if (refresh)
        {
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

            int fullWhiskeyNum = 0;
            int normalWhiskeyNum = 0;
            int oldWhiskeyNum = 0;
            int purses = 0;
            int strongboxes = 0;
            int rubies = 0;

            foreach (GameItem item in g)
            {
                switch (item.getType())
                {
                    case ItemType.Purse:
                        purses++;
                        break;
                    case ItemType.Ruby:
                        rubies++;
                        break;
                    case ItemType.Strongbox:
                        strongboxes++;
                        break;
                    case ItemType.Whiskey:
                        
                        switch(((Whiskey)item).getWhiskeyKind()) {
                            case WhiskeyKind.Unknown:
                                fullWhiskeyNum++;
                                break;
                            case WhiskeyKind.Normal:
                                normalWhiskeyNum++;
                                break;
                            case WhiskeyKind.Old:
                                oldWhiskeyNum++;
                                break;
                        }

                        break;

                }
            }

            string value = "";
            int num = 0;
            foreach (Transform t in lootPosition.transform)
            {
                switch (t.name)
                {
                    case "FullWhiskey":
                        t.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", fullWhiskeyNum);

                        if (fullWhiskeyNum <= 0)
                        {
                            t.gameObject.SetActive(false);
                        }
                        break;
                    case "OldWhiskey":
                        t.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", oldWhiskeyNum);

                        if (oldWhiskeyNum <= 0)
                        {
                            t.gameObject.SetActive(false);
                        }
                        break;
                    case "NormalWhiskey":
                        t.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", normalWhiskeyNum);

                        if (normalWhiskeyNum <= 0)
                        {
                            t.gameObject.SetActive(false);
                        }
                        break;
                    case "Strongbox":
                        t.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", strongboxes);

                        if (strongboxes <= 0)
                        {
                            t.gameObject.SetActive(false);
                        }
                        break;
                    case "Ruby":
                        t.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", rubies);

                        if (rubies <= 0)
                        {
                            t.gameObject.SetActive(false);
                        }
                        break;
                    case "Purse":
                        t.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = String.Format("x{0}", purses);

                        if (purses <= 0)
                        {
                            t.gameObject.SetActive(false);
                        }
                        break;
                }
            }

            Debug.Log("[UpdateLootAtLocationListener] Loot refreshed.");
        } else
        {
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
}
