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
            GameObject adjacentCar = GameUIManager.gameUIManagerInstance.getTrainCar(atIndex);
            Vector3 coordinates = new Vector3(adjacentCar.transform.position.x, this.transform.position.y, this.transform.position.z);

            this.transform.position = coordinates;

            atIndex++;
        }
        
    }
}
