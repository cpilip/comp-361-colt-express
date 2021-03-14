using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using GameUnitSpace;
using CardSpace;
using PositionSpace;
using RoundSpace;
using Newtonsoft.Json.Linq;

public class CommunicationAPIHandler : MonoBehaviour
{
    private static KnownTypesBinder knownTypesBinder = new KnownTypesBinder
    {
        KnownTypes = new List<Type> {
            typeof(Card),
            typeof(ActionCard),
            typeof(BulletCard),
            typeof(GameItem),
            typeof(GameUnit),
            typeof(GameStatus),
            typeof(Marshal),
            typeof(Player),
            typeof(Position),
            typeof(Round),
            typeof(TrainCar),
            typeof(Turn)
        }
    };

    //Will include object type in JSON string
    public static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        SerializationBinder = knownTypesBinder
    };

    //action is the action you wish to execute on the client (list will be provided)
    //Then add each object for the message as a parameter (e.g. wanting to send a Turn t and a Round r, so we do sendTocClient(doSomething, t, r) and so on)
    public static void getMessageFromServer(string data)
    {

        List<System.Object> t = JsonConvert.DeserializeObject<List<System.Object>>(data, settings);
        t.RemoveAt(0);
        
        string d = JsonConvert.SerializeObject(t, settings);
        string eventName = JArray.Parse(data)[0].ToObject<string>();

        //Debug.Log(eventName + " " + d);
        EventManager.TriggerEvent(eventName, d);


    }
}

//Clean up type formatting
public class KnownTypesBinder : ISerializationBinder
{
    public IList<Type> KnownTypes { get; set; }

    public Type BindToType(string assemblyName, string typeName)
    {
        return KnownTypes.SingleOrDefault(t => t.Name == typeName);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;
        typeName = serializedType.Name;
    }
}

public enum GameStatus
{
    ChoosingBandits,
    Schemin,
    Stealin,
    FinalizingCard,
    Completed
}
