using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerServerButton : MonoBehaviour
{
    public void doServer(string msg)
    {
        EventManager.EventManagerInstance.eventManagerLocation.GetComponent<NamedClient>().SendMessageToServer("H");
    }
}
