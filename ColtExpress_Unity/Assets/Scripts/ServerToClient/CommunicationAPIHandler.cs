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
            typeof(Shotgun),
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

    public class CardConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Card));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["$type"].Value<string>() == "ActionCard")
                return jo.ToObject<ActionCard>(serializer);

            if (jo["$type"].Value<string>() == "BulletCard")
                return jo.ToObject<BulletCard>(serializer);

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


    }

    public class GameUnitConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(GameUnit));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["$type"].Value<string>() == "Marshal")
                return jo.ToObject<Marshal>(serializer);

            if (jo["$type"].Value<string>() == "Shotgun")
                return jo.ToObject<Shotgun>(serializer);

            if (jo["$type"].Value<string>() == "Player")
                return jo.ToObject<Player>(serializer);

            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


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