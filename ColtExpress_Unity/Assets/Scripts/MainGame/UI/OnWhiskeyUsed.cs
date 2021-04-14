using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Author: Christina Pilip
 * Usage: Variant for whiskey buttons. Because of the way that the coroutine is set up, this is necessary. 
 */
public class OnWhiskeyUsed : MonoBehaviour
{
    public delegate void wasWhiskeyUsed();
    public event wasWhiskeyUsed notifyWhiskeyWasUsed;

    public bool thisWhiskeyTypeUsed = false;
    public bool allowedForPunch = false;

    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(TaskOnClick);
        
    }
    public void TaskOnClick()
    {
        if (allowedForPunch)
        {
            allowedForPunch = false;
            GameUIManager.gameUIManagerInstance.gameObject.GetComponent<StealinPhaseManager>().playerChoseLootPunch();
        } else
        {
            thisWhiskeyTypeUsed = true;
            notifyWhiskeyWasUsed?.Invoke();
        }
    }


}
