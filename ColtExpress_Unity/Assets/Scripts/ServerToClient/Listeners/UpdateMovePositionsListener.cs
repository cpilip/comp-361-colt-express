using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateMovePositionsListener : UIEventListenable
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

        Debug.Log(data);

        foreach (int index in i)
        {
            GameObject gamePosition = GameUIManager.gameUIManagerInstance.getTrainCarPosition(index, positions[positionsIndex].isRoof());
            Image image = gamePosition.GetComponent<Image>();

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.392f);

            gamePosition.GetComponent<Button>().enabled = true;
            positionsIndex++;

        }
        Debug.Log("[UpdateMovePositionsListener] Moves now visible.");
        
    }
}
