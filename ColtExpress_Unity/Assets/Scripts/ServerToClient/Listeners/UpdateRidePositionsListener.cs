using GameUnitSpace;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateRidePositionsListener : UIEventListenable
{
    /*
     *  {
            eventName = action,
            positions = (List<Position>)args[0],
            indices = (List<int>)args[1]
        };
     */
    public override void updateElement(string data)
    {
        JObject o = JObject.Parse(data);
        List<int> i = o.SelectToken("indices").ToObject<List<int>>();
        List<Position> positions = o.SelectToken("positions").ToObject<List<Position>>();
        Character c = o.SelectToken("player").ToObject<Character>();
        int playerAtIndex = o.SelectToken("playerAtIndex").ToObject<int>();

        int positionsIndex = 0;

        GameObject horseSet = null;
        if (playerAtIndex == -1)
        {
            horseSet = GameUIManager.gameUIManagerInstance.getHorseSet(MoveStageCoachListener.atIndex);
        } else
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
                    Debug.LogError("Free horse " + horsePosition.name);
                    break;
                }
            }
        }

        GameUIManager.gameUIManagerInstance.getCharacterObject(c).transform.parent = horsePosition.transform;

        foreach (int index in i)
        {
            GameObject gamePosition = null;
            //Stagecoach roof
            if (index == -1)
            {
                gamePosition = GameUIManager.gameUIManagerInstance.getStagecoachPosition(positions[positionsIndex].isRoof());

                if (positions[positionsIndex].isRoof())
                {
                    gamePosition.GetComponent<Image>().raycastTarget = true;
                }
            }
            else
            {
                gamePosition = GameUIManager.gameUIManagerInstance.getTrainCarPosition(index, positions[positionsIndex].isRoof());
            }
            Image image = gamePosition.GetComponent<Image>();

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.392f);

            gamePosition.GetComponent<Button>().enabled = true;
            positionsIndex++;

        }
        Debug.Log("[UpdateRidePositionsListener] Moves now visible and on a horse.");
    }
}
