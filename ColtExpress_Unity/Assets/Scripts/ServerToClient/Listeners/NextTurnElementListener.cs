using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class NextTurnElementListener : UIEventListenable
{
    private static int _currentTurn;
    
    private void Awake()
    {
        _currentTurn = 0;
    }

    public override void updateElement(string data)
    { 
        //JSON String parse somewhere here instead
        _currentTurn = Int32.Parse(data);
        //Debug.Log(_currentTurn);
        
        //FFEE6D
        this.transform.GetChild(_currentTurn).GetComponent<Image>().color = new Color(1.000f, 0.933f, 0.427f, 0.914f);


        _currentTurn = (_currentTurn == 0) ? (this.transform.childCount - 1) : _currentTurn - 1;
        
        this.transform.GetChild(_currentTurn).GetComponent<Image>().color = new Color(1.000f, 1f, 1f, 1f);

        //Debug.Log("Updated image element of next turn listener");
    }

}
