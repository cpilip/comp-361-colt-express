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
     * Usage: Client to server communications
     * 
     * Call CommunicationAPI.sendMessageToServer(event, var args). List each object, primitive, or enum you want to send as a new parameter.
     * Transmit the resulting string to the client.
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

        //action is the action you wish to execute on the client (list will be provided)
        //Then add each object for the message as a parameter (e.g. wanting to send a Turn t and a Round r, so we do sendTocClient(doSomething, t, r) and so on)
        public static void sendMessageToServer(params object[] args)
        {
            //List<System.Object> objectsToSerialize = new List<System.Object>();
            //objectsToSerialize.AddRange(args);

            //var testType = args[0].GetType().MakeGenericType();

            //dynamic type = args[0].GetType().GetProperty("Value").GetValue(args[0], null);

            string data = JsonConvert.SerializeObject(args[0], settings);

            //Send to server

            //Debug.Log(data);
            EventManager.EventManagerInstance.GetComponent<NamedClient>().SendMessageToServer(data);
        }
        public static void sendMessageToServer(Character c)
        {
            //List<System.Object> objectsToSerialize = new List<System.Object>();
            //objectsToSerialize.AddRange(args);

            //var testType = args[0].GetType().MakeGenericType();

            //dynamic type = args[0].GetType().GetProperty("Value").GetValue(args[0], null);

            string data = JsonConvert.SerializeObject(c, settings);

            //Send to server

            //Debug.Log(data);
            EventManager.EventManagerInstance.GetComponent<NamedClient>().SendMessageToServer(data);
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

