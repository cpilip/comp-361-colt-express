using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSelectHostageListener : UIEventListenable
{
    
    public override void updateElement(string data)
    {
        this.GetComponent<StealinPhaseManager>().chooseHostage();

        Debug.Log("[UpdateSelectHostageListener] Choosing hostage.");
    }
}
