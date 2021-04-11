using GameUnitSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCarHasAHorseListener : UIEventListenable
{
    /*var definition = new
            {
                eventName = action,
                indexOfTrainCar = (int)args[0]
    player = Character
            };*/
    public override void updateElement(string data)
    {
        JObject o = JObject.Parse(data);
        
        int indexOfTrainCar = o.SelectToken("indexOfTrainCar").ToObject<int>();
        Character player = o.SelectToken("player").ToObject<Character>();

        GameObject horse = GameUIManager.gameUIManagerInstance.getHorseByCharacter(player);
        GameObject oldHorseSet = horse.transform.parent.gameObject;
        GameObject horseSetToGoTo = GameUIManager.gameUIManagerInstance.getHorseSet(indexOfTrainCar);

        horse.transform.parent = horseSetToGoTo.transform;
        //Make sure the horse is at the top
        horse.transform.SetAsFirstSibling();
        //Adjust spacing of old and new horseset
        GameUIManager.gameUIManagerInstance.adjustHorseSpacing(horseSetToGoTo);
        GameUIManager.gameUIManagerInstance.adjustHorseSpacing(oldHorseSet);

        Debug.Log("[UpdateCarHasAHorseListener] Moved horse to set at " + indexOfTrainCar);
    }
    
}
