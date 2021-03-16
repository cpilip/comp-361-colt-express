using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Sockets;

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
    
    
    public static void sendMessageToClient(TcpClient cli, string action, params System.Object[] args)
    {
        if (action == "updateTrain")
        {
            List<TrainCar> t = (List<TrainCar>)args[0];
            
            int i = 0;
            foreach (TrainCar n in t)
            {
                var definition = new {
                    eventName = action,
                    indexofCar = i,
                    i_items = n.getInside().getUnits_items(),
                    i_players = n.getInside().getUnits_players(),
                    r_items = n.getRoof().getUnits_items(),
                    r_players = n.getRoof().getUnits_players(),
                };
                i++;
                MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
            }
            return;
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
                MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
                
            }
            return;
        }
        else if (action == "updatePlayerHand")
        {
            int i = (int)args[0];
            List<Card> c = (List<Card>)args[1];
            List<ActionKind> l  = new List<ActionKind>();

            c.OfType<ActionCard>().ToList().ForEach(t => l.Add(t.getKind()));

            var definition = new
            {
                eventName = "updatePlayerHand",
                currentPlayerIndex = i,
                cardsToAdd = l 
            };
           
        } else if (action == "updateGameStatus")
        {
            var definition = new
            {
                eventName = "updateGameStatus",
                statusIs = (bool)args[0],
            };

        }
        else if (action == "updateCurrentRound")
        {
            Round r = (Round)args[0];
            List<TurnType> l = new List<TurnType>();

            r.getTurns().ForEach(t => l.Add(t.getType()));

            var definition = new
            {
                eventName = "updateCurrentRound",
                //anEvent = r.getEvent();
                isLastRound = r.getIsLastRound(),
                turns = l
            };

        }
        else if (action == "updateCurrentTurn")
        {
            int i = (int)args[0];

            var definition = new
            {
                eventName = "updateCurrentTurn",
                currentTurn = i
            };

        } else if (action == "updateWaitingForInput")
            {
                Character c = (Character)args[0];

            var definition = new
            {
                eventName = "updateWaitingForInput",
                currentPlayer = c
            };

        } 

        List<System.Object> objectsToSerialize = new List<System.Object>();
        objectsToSerialize.Add(action);
        objectsToSerialize.AddRange(args);

        if (cli == null) {
            //Serialize parameters as a array with first element being the action
            MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

        } else {
            //Serialize parameters as a array with first element being the action
            MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
        }
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


