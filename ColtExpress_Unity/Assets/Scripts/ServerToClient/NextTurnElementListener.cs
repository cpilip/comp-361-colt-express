using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NextTurnElementListener : UIEventListenable
{
    private static int iterator;
    
    private void Awake()
    {
        iterator = 0;
    }

    public override void updateElement()
    {
        if (iterator == transform.childCount)
        {
            iterator = 0;
        }

        //FFEE6D
        Debug.Log(iterator);
        this.transform.GetChild(iterator).GetComponent<Image>().color = new Color(1.000f, 0.933f, 0.427f, 0.914f);
        
        Debug.Log((iterator - 1) % this.transform.childCount);
        this.transform.GetChild(mod(iterator - 1, this.transform.childCount)).GetComponent<Image>().color = new Color(1.000f, 1f, 1f, 1f);
        
        iterator = mod(iterator + 1,  this.transform.childCount);

        Debug.Log("Updated image element of next turn listener");
    }

    private int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
