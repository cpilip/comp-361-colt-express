using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGControl : MonoBehaviour
{
    public GameObject turnMenu;
    public GameObject boardBlocker;

    public void enableTurnMenu()
    {
        if (turnMenu != null)
        {
            turnMenu.SetActive(true);
        }
    }

    public void enableBoard()
    {
        if (boardBlocker != null)
        {
            boardBlocker.SetActive(false);
        }
    }
}
