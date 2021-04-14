using AttackSpace;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHorseAttackListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        /*{
            eventName = action,
                list of attack positions
            };
        */
        JObject o = JObject.Parse(data);
        List<AttackPosition> positions = o.SelectToken("positions").ToObject<List<AttackPosition>>();
        
        //If at at train car before locomotive, auto enter

        foreach (AttackPosition p in positions)
        {
            GameObject charObject = GameUIManager.gameUIManagerInstance.getCharacterObject(p.GetCharacter());

            //Convert position to work with set - server returns 0, 1.. with 0 as caboose but 0 is locomotive
            int pos = (p.getPosition() == 0) ? positions.Count : positions.Count + 1 - 1 - p.getPosition();

            //Debug.LogError(pos);
         

            if (p.hasStopped() == false)
            {
                //If the horse has stopped, parent the character under the train car interior and set the horse to the same pos
                charObject.transform.parent = GameUIManager.gameUIManagerInstance.getTrainCarPosition(pos, false).transform;

                //Get the horse and add it to the next horse set
                GameObject horse = GameUIManager.gameUIManagerInstance.getHorseByCharacter(p.GetCharacter());

                GameObject horseOriginalSet = horse.transform.parent.gameObject;

                horse.transform.parent = GameUIManager.gameUIManagerInstance.getHorseSet(pos).transform;

                //Adjust spacing of old and new horseset
                GameUIManager.gameUIManagerInstance.adjustHorseSpacing(horseOriginalSet);
                GameUIManager.gameUIManagerInstance.adjustHorseSpacing(horse.transform.parent.gameObject);
                //Make sure the horse is at the top
                horse.transform.SetAsFirstSibling();
            } 
            else
            {
                //Get the horse and add it to the next horse set
                GameObject horse = GameUIManager.gameUIManagerInstance.getHorseByCharacter(p.GetCharacter());

                GameObject horseOriginalSet = horse.transform.parent.gameObject;

                horse.transform.parent = GameUIManager.gameUIManagerInstance.getHorseSet(pos).transform;

                //Adjust spacing of old and new horseset
                GameUIManager.gameUIManagerInstance.adjustHorseSpacing(horseOriginalSet);
                GameUIManager.gameUIManagerInstance.adjustHorseSpacing(horse.transform.parent.gameObject);
                //Make sure the horse is at the top
                horse.transform.SetAsFirstSibling();
            }
        }

        
        Debug.Log("[UpdateHorseAttackListener] Resolved attack positions.");
    }
}
