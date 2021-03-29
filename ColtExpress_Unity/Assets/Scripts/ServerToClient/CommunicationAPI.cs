using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CardSpace;
using GameUnitSpace;
using PositionSpace;
using RoundSpace;
using UnityEngine;

namespace ClientCommunicationAPI
{

    /* Author: Christina Pilip
     * Usage: Client to server communications.
     * 
     * The only important part is to call sendMessageToServer(var anonymous class). Each anonymous class should be defined in the appropriate listener calling sendMessageToServer in this format:
     * 
     * var definition = 
     * { eventName = "myEventName",
     * propertyName = ...
     * }
     * 
     */

    public class CommunicationAPI
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
        private static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            SerializationBinder = knownTypesBinder
        };

        //Serialize the provided arguments and send to the server
        public static void sendMessageToServer(params object[] args)
        {
            string data = JsonConvert.SerializeObject(args[0], settings);
            EventManager.EventManagerInstance.GetComponent<NamedClient>().sendToServer(data);
        }

        public static void sendMessageToServer(Character c)
        {
            string data = JsonConvert.SerializeObject(c, settings);
            EventManager.EventManagerInstance.GetComponent<NamedClient>().sendToServer(data);
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

