using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundEventListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        int numCars = GameUIManager.gameUIManagerInstance.getNumTrainCars();
        int carIndex = numCars--;

        //At caboose

        JObject o = JObject.Parse(data);
        string roundEvent = o.SelectToken("roundEvent").ToObject<string>();

        if (roundEvent == "HigherSpeed")
        {
            while (carIndex > 0)
            {
                GameObject currentHorseSet = GameUIManager.gameUIManagerInstance.getHorseSet(carIndex);
                GameObject beforeHorseSet;
                if (carIndex - 1 == 0)
                {
                    beforeHorseSet = GameUIManager.gameUIManagerInstance.getHorseSet(0);
                }
                else
                {
                    beforeHorseSet = GameUIManager.gameUIManagerInstance.getHorseSet(carIndex - 1);
                }


                carIndex--;

                //Get numHorses (remember button)
                int numHorses = beforeHorseSet.transform.childCount - 1;
                while (numHorses > 0)
                {
                    GameObject firstHorse = beforeHorseSet.transform.GetChild(0).gameObject;
                    firstHorse.transform.parent = currentHorseSet.transform;
                    numHorses--;
                }
            }
        }
        else if (roundEvent == "Escape")
        {
            Character c = o.SelectToken("whoEscaped").ToObject<Character>();
            int playerAtIndex = o.SelectToken("playerAtIndex").ToObject<int>();

            GameObject horseSet = null;
            if (playerAtIndex == -1)
            {
                horseSet = GameUIManager.gameUIManagerInstance.getHorseSet(MoveStageCoachListener.atIndex);
            }
            else
            {
                horseSet = GameUIManager.gameUIManagerInstance.getHorseSet(playerAtIndex);
            }

            GameObject horsePosition = null;
            //If there is a horse here to ride
            if (horseSet.transform.childCount > 2)
            {
                for (int j = 0; j < horseSet.transform.childCount - 1; j++)
                {
                    if (horseSet.transform.GetChild(j).GetChild(0).childCount == 0)
                    {
                        //Empty horse found
                        horsePosition = horseSet.transform.GetChild(j).GetChild(0).gameObject;
                        //Debug.LogError("Free horse " + horsePosition.name);
                        break;
                    }
                }
            }
            else if (horseSet.transform.childCount == 2)
            {
                if (horseSet.transform.GetChild(0).GetChild(0).childCount == 0)
                {
                    //Empty horse found
                    horsePosition = horseSet.transform.GetChild(0).GetChild(0).gameObject;
                    //Debug.LogError("Free horse " + horsePosition.name);
                }
            }

            GameUIManager.gameUIManagerInstance.getCharacterObject(c).transform.parent = horsePosition.transform;
            GameUIManager.gameUIManagerInstance.remapCharacterAndHorse(c, horsePosition);

            Destroy(horsePosition.transform.parent);

        }
        else if (roundEvent == "PantingHorses")
        {
            GameObject currentHorseSet = GameUIManager.gameUIManagerInstance.getHorseSet(carIndex);
            List<GameObject> horses = new List<GameObject>();
            
            int horsesRemoved = 0;

            int remainingHorseCount = GameUIManager.gameUIManagerInstance.getTotalNumOfHorses();

            while (carIndex > 0)
            {
                int numHorses = currentHorseSet.transform.childCount - 1;

                if (numHorses == 1)
                {
                    horses.Add(currentHorseSet.transform.GetChild(0).gameObject);
                    horsesRemoved++;

                    if (horsesRemoved == 2 || horsesRemoved == remainingHorseCount)
                    {
                        break;
                    }
                }
                else if (numHorses >= 2)
                {
                    horses.Add(currentHorseSet.transform.GetChild(0).gameObject);
                    horsesRemoved++;

                    if (horsesRemoved == 2 || horsesRemoved == remainingHorseCount)
                    {
                        break;
                    }
                    else
                    {
                        horses.Add(currentHorseSet.transform.GetChild(0).gameObject);
                        horsesRemoved++;

                        if (horsesRemoved == 2 || horsesRemoved == remainingHorseCount)
                        {
                            break;
                        }
                    }
                }

                carIndex--;
                currentHorseSet = GameUIManager.gameUIManagerInstance.getHorseSet(carIndex);
            }

            if (horses.Count == 2)
            {
                Destroy(horses[0]);
                Destroy(horses[1]);
            } else if (horses.Count == 1)
            {
                Destroy(horses[0]);
            }
                    
        }
    }
}

  