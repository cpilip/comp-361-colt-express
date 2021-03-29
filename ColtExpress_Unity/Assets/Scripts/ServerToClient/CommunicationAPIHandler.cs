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
using System.IO;
using System.Text.RegularExpressions;

namespace ClientCommunicationAPIHandler
{
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
            typeof(Turn),
            typeof(Character)
        }
        };

        //Will include object type in JSON string
        public static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        //Figure out what event to trigger from the JSON messafe
        //We only parse the eventName property, a default - the corresponding listener will know and parse the other properties in the messsage.
        public static void getMessageFromServer(string data)
        {
            //Debug.Log("[Handler] RECEIVED: " + data);
            JObject o = JObject.Parse(data);
            string eventName = o.SelectToken("eventName").ToString();

            Debug.Log("[Handler] TRIGGERING: " + eventName);
            //Debug.Log(eventName);
            EventManager.TriggerEvent(eventName, data);
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

}

public enum GameStatus
{
    Schemin,
    Stealin
}