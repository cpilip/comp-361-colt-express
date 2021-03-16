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
        //PRE: data = TrainCar
        JObject o = JObject.Parse(data);
        TrainCar car = o.SelectToken("car").ToObject<TrainCar>();

        //GameUIManager.gameControllerInstance.createTrainCarObject(, );

        foreach (Player p in car.getInside().getPlayers())
        {
            GameUIManager.gameControllerInstance.createCharacterObject(p.getBandit());
        }
        
        car.getInside().getPlayers();
        car.getInside().getItems();
        //System.Object[] deserializedProduct = JsonConvert.DeserializeObject<System.Object[]>(data, CommunicationAPIHandler.settings);
        Debug.Log("Making TRAIN!");


    }
}
