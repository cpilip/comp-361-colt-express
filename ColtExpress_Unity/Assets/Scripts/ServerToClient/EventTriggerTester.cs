using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerTester : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            EventManager.TriggerEvent("updateRound");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            EventManager.TriggerEvent("updateTurns");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            EventManager.TriggerEvent("updateNextTurn");
        }
    }
}
