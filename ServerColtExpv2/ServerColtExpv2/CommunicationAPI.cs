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
using Newtonsoft.Json.Linq;

/* Author: Christina Pilip
 * Usage: Server to client communication (for now)
 * 
 * Call CommunicationAPI.sendMessageToClient(event, var args). List each object, primitive, or enum you want to send as a new parameter (ex. sendMessageToClient(doSomething, Round r, Turn t); ).\
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
            typeof(Turn)
        }
    };

    //Will include object type in JSON string
    private static JsonSerializerSettings settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        SerializationBinder = knownTypesBinder
    };

    //action is the action you wish to execute on the client (list will be provided)
    //Then add each object for the message as a parameter (e.g. wanting to send a Turn t and a Round r, so we do sendTocClient(doSomething, t, r) and so on)
    
    
    public static void sendMessageToClient(string action, params System.Object[] args)
    {
        if (action == "updateTrain")
        {
            List<TrainCar> t = (List<TrainCar>)args[0];
            
            int i = 0;
            foreach (TrainCar n in t)
            {
                var definition = new { eventName = action, car = n };
                MyTcpListener.sendToClient(JsonConvert.SerializeObject(definition, settings));
            }
        } else if (action == "updatePlayers")
        {
            List<Player> t = (List<Player>)args[0];

            foreach (Player n in t)
            {
                var definition = new {
                    eventName = "updatePlayer",
                    player = n.getBandit(),
                    h_ActionCards = n.getHand_actionCards(),
                    h_BulletCards = n.getHand_bulletCards(),
                    d_ActionCards = n.getDiscard_actionCards(),
                    d_BulletCards = n.getDiscard_bulletCards()
                };
                MyTcpListener.sendToClient(JsonConvert.SerializeObject(definition, settings));
            }
        }

        List<System.Object> objectsToSerialize = new List<System.Object>();
        objectsToSerialize.Add(action);
        objectsToSerialize.AddRange(args);

        //Serialize parameters as a array with first element being the action
        MyTcpListener.sendToClient(JsonConvert.SerializeObject(objectsToSerialize, settings));
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


