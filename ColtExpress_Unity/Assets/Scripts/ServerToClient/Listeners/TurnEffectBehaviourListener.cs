using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TurnEffectBehaviourListener : UIEventListenable
{
    private static TurnType _currentTurn;

    public override void updateElement(string data)
    {
        TurnType _currentTurn = JsonConvert.DeserializeObject<TurnType>(data);

        switch (_currentTurn)
        {
            case TurnType.SpeedingUp:
                Debug.Log("Is a Speeding Up turn");
                break;
            case TurnType.Standard:
                Debug.Log("Is a Standard turn");
                break;
            case TurnType.Tunnel:
                Debug.Log("Is a Tunnel turn");
                break;
            case TurnType.Switching:
                Debug.Log("Is a Switching turn");
                break;
            default:
                Debug.LogError("DATA was not a TurnType.");
                break;
        } 
    }
}
