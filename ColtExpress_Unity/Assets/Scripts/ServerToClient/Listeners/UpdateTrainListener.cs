using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTrainListener : UIEventListenable
{
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
        int i = o.SelectToken("indexOfCar").ToObject<int>();

        //GameUIManager.gameControllerInstance.createTrainCarObject(, );

        GameUIManager.gameControllerInstance.createTrainCarObject(i);

        foreach (Player p in car.getInside().getPlayers())
        {
            GameUIManager.gameControllerInstance.createCharacterObject(p.getBandit());
        }
        
        

    }
}
