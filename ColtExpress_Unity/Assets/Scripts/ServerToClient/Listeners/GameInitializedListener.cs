using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializedListener : UIEventListenable
{
    public GameObject gameInitializationBlocker;
    public override void updateElement(string data)
    {

        StopCoroutine("AnimateEllipsis");
        gameInitializationBlocker.SetActive(false);
        Destroy(gameInitializationBlocker);
    }
}
