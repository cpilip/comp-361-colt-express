using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmitClientStateListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        GameUIManager.gameUIManagerInstance.gameObject.GetComponent<SaveManager>().createSaveGame();

        //Transmite file
    }
}
