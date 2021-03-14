using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPlayerElementListener : UIEventListenable
{
    private static int _currentPlayer;

    private void Awake()
    {
        _currentPlayer = 0;
    }

    public override void updateElement(string currentTurn)
    {
        //JSON String parse somewhere here instead
        _currentPlayer = Int32.Parse(currentTurn);
        //Debug.Log(_currentPlayer);

        //FFEE6D
        this.transform.GetChild(_currentPlayer).GetComponent<Image>().color = new Color(1.000f, 0.933f, 0.427f, 0.914f);

        _currentPlayer = (_currentPlayer == 0) ? (this.transform.childCount - 1) : _currentPlayer - 1;

        this.transform.GetChild(_currentPlayer).GetComponent<Image>().color = new Color(1.000f, 1f, 1f, 1f);


        //Debug.Log("Updated image element of next turn listener");
    }
}
