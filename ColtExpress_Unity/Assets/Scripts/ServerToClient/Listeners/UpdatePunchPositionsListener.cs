using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePunchPositionsListener : UIEventListenable
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
        int positionsIndex = 0;

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
        Debug.Log("[UpdatePunchPositionsListener] Moves now visible.");

    }
}

