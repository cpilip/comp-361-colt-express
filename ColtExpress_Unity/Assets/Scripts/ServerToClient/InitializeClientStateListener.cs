using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeClientStateListener : UIEventListenable
{
    public override void updateElement(string data)
    {
        //Receive client state from server
        GameUIManager.gameUIManagerInstance.gameObject.GetComponent<SaveManager>().loadSavedGame(data);
    }
}
