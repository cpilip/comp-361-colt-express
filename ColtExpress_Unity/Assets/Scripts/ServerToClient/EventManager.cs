using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

/* Author: Christina Pilip
 * Usage: An Event manager singleton for allowing UI items to subscribe to events (messages transmitted from the server).
 */
[System.Serializable]
public class Event : UnityEvent<String> { }
public class EventManager : MonoBehaviour
{
    //Object that the EventManager script is on
    public GameObject eventManagerLocation;

    private Dictionary<string, Event> eventMap;

    //EventManager instance
    private static EventManager eventManager;

    public static EventManager EventManagerInstance
    {
        get
        {
            return eventManager;
        }
    }

    void Awake()
    {
        if (!eventManager)
        {
            //Obtain the EventManager instance
            eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

            //Initialize the EventManager
            if (eventManager == null)
            {
                Debug.LogError("EventManager failed to initialize.");
            }
            else
            {
                eventManager.Init();
            }
        }

        DontDestroyOnLoad(eventManagerLocation);
    }

    //Initialize the Event Manager map
    void Init()
    {
        if (eventMap == null)
        {
            eventMap = new Dictionary<string, Event>();
        }
    }

    //Add a listener for an event with name eventName
    public static void StartListening(string eventName, UnityAction<String> listener)
    {
        if (eventManager == null) { return; }

        Event thisEvent = null;
        //Add the listener to the event if it exists
        if (EventManagerInstance.eventMap.TryGetValue(eventName, out thisEvent))
        {
            
            thisEvent.AddListener(listener);
        }
        //Make the event if it does not exist
        else
        {
            thisEvent = new Event();
            thisEvent.AddListener(listener);
            EventManagerInstance.eventMap.Add(eventName, thisEvent);
        }
    }

    //Remove a listener for an event with name eventName
    public static void StopListening(string eventName, UnityAction<String> listener)
    {
        if (eventManager == null) { return; }

        Event thisEvent = null;
        //Remove the listern from the event if it exists
        if (EventManagerInstance.eventMap.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    //Invoke the corresponding event's function
    public static void TriggerEvent(string eventName, string data = null)
    {
        Event thisEvent = null;
        if (EventManagerInstance.eventMap.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(data);
        }
    }
}

