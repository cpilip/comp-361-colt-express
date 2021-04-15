using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStageCoachListener : UIEventListenable
{
    public static int atIndex = 0;

    public override void updateElement(string data)
    {
        if (atIndex < GameUIManager.gameUIManagerInstance.getNumTrainCars())
        {
            atIndex++;
        }
        if (atIndex < GameUIManager.gameUIManagerInstance.getNumTrainCars())
        {
            GameObject adjacentCar = GameUIManager.gameUIManagerInstance.getTrainCar(atIndex);
            Vector3 coordinates = new Vector3(adjacentCar.transform.position.x, this.transform.position.y, this.transform.position.z);

            this.transform.position = coordinates;

            GameObject stagecoachLootRoof = GameUIManager.gameUIManagerInstance.getStageCoachLoot(true);
            coordinates = new Vector3(coordinates.x, stagecoachLootRoof.transform.position.y, stagecoachLootRoof.transform.position.z);
            stagecoachLootRoof.transform.position = coordinates;

            GameObject stagecoachLootInterior = GameUIManager.gameUIManagerInstance.getStageCoachLoot(false);
            coordinates = new Vector3(coordinates.x, stagecoachLootInterior.transform.position.y, stagecoachLootInterior.transform.position.z);
            stagecoachLootInterior.transform.position = coordinates;


        }

        Debug.Log("[MoveStageCoachListener] Stagecoach moved.");
        
    }
}
