using GameUnitSpace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PositionSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGameUnitListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /* PRE: data
        * 
            {
                eventName = action,
                gameUnit = (GameUnit)args[0],
                position = p,
                isInStageCoach = (p.getTrainCar() is StageCoach) ? true : false,
                trainCarIndex = (int)args[2]
            };
        */
        JObject o = JObject.Parse(data);
        int i = o.SelectToken("trainCarIndex").ToObject<int>();

        bool inStagecoach = o.SelectToken("isInStageCoach").ToObject<bool>();

        if (i == -1)
        {
            
            if (inStagecoach)
            {
                Position p = o.SelectToken("position").ToObject<Position>();
                GameObject gamePosition = GameUIManager.gameUIManagerInstance.getStagecoachPosition(p.isRoof());
                string location = (p.isRoof()) ? "ROOF" : "INSIDE";

                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new ClientCommunicationAPIHandler.GameUnitConverter());

                //Deserialize the GameUnit
                GameUnit c = o.SelectToken("gameUnit").ToObject<GameUnit>(serializer);

                //Handle appropriately
                if (c.GetType() == typeof(Player))
                {
                    //Get the player's bandit object
                    Player pl = (Player)c;
                    GameObject gameCharacterObject = GameUIManager.gameUIManagerInstance.getCharacterObject(pl.getBandit());

                    //Parent it under the new position instead
                    gameCharacterObject.transform.parent = gamePosition.transform;

                    Debug.LogError("[MoveGameUnitListener] Moved " + ((Player)c).getBandit() + " to stagecoach " + location);
                }
                else if (c.GetType() == typeof(Marshal))
                {
                    //Get the marshal object
                    GameObject gameCharacterObject = GameUIManager.gameUIManagerInstance.getCharacterObject(Character.Marshal);

                    //Parent it under the new position instead
                    gameCharacterObject.transform.parent = gamePosition.transform;
                    Debug.LogError("[MoveGameUnitListener] Moved " + Character.Marshal.ToString() + " to stagecoach " + location);
                }
                else if (c.GetType() == typeof(Shotgun))
                {
                    //Get the shotgun object
                    GameObject gameCharacterObject = GameUIManager.gameUIManagerInstance.getCharacterObject(Character.Shotgun);

                    //Parent it under the new position instead
                    gameCharacterObject.transform.parent = gamePosition.transform;
                    Debug.LogError("[MoveGameUnitListener] Moved " + Character.Shotgun.ToString() + " to stagecoach " + location);
                }
            } else
            {
                Debug.LogError("[MoveGameUnitListener] DOES NOT HANDLE HORSE POSITIONS CURRENTLY.");
            }

        } else
        {

            Position p = o.SelectToken("position").ToObject<Position>();
            GameObject gamePosition = GameUIManager.gameUIManagerInstance.getTrainCarPosition(i, p.isRoof());
            string location = (p.isRoof()) ? "ROOF" : "INSIDE";

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new ClientCommunicationAPIHandler.GameUnitConverter());

            //Deserialize the GameUnit
            GameUnit c = o.SelectToken("gameUnit").ToObject<GameUnit>(serializer);

            //Handle appropriately
            if (c.GetType() == typeof(Player))
            {
                //Get the player's bandit object
                Player pl = (Player)c;
                GameObject gameCharacterObject = GameUIManager.gameUIManagerInstance.getCharacterObject(pl.getBandit());

                //Parent it under the new position instead
                gameCharacterObject.transform.parent = gamePosition.transform;

                Debug.LogError("[MoveGameUnitListener] Moved " + ((Player)c).getBandit() + " to train car " + i + " " + location);
            }
            else if (c.GetType() == typeof(Marshal))
            {
                //Get the marshal object
                GameObject gameCharacterObject = GameUIManager.gameUIManagerInstance.getCharacterObject(Character.Marshal);

                //Parent it under the new position instead
                gameCharacterObject.transform.parent = gamePosition.transform;
                Debug.LogError("[MoveGameUnitListener] Moved " + Character.Marshal.ToString() + " to train car " + i + " " + location);
            }
            else if (c.GetType() == typeof(Shotgun))
            {
                //Get the shotgun object
                GameObject gameCharacterObject = GameUIManager.gameUIManagerInstance.getCharacterObject(Character.Shotgun);

                //Parent it under the new position instead
                gameCharacterObject.transform.parent = gamePosition.transform;
                Debug.LogError("[MoveGameUnitListener] Moved " + Character.Shotgun.ToString() + " to train car " + i + " " + location);
            }
        }

    }
}
