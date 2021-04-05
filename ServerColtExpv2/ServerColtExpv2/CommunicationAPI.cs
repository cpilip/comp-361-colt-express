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
            //"updateTrain" triggers the initialization of the train
            List<TrainCar> t = (List<TrainCar>)args[0];
            
            int i = 0;
            foreach (TrainCar n in t)
            {
                var definition = new {
                    eventName = action,
                    indexofCar = i,
                    i_items = n.getInside().getUnits_items(),
                    i_players = n.getInside().getUnits_players(),
                    i_hasMarshal = n.getInside().hasMarshal(Marshal.getInstance()),

                    r_items = n.getRoof().getUnits_items(),
                    r_players = n.getRoof().getUnits_players(),
                    r_hasMarshal = n.getRoof().hasMarshal(Marshal.getInstance())
                };

                i++;

                if (cli == null)
                {
                    //Serialize parameters as a array with first element being the action
                    MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

                }
                else
                {
                    //Serialize parameters as a array with first element being the action
                    MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
                }
            }
        } else if (action == "updatePlayers")
        {
            //"updatePlayers" triggers creation of player profiles for every player in the list provided;
            //if the current player in the list is the player of the client, initialize their hand, discard pile, and remaining bullets additionally
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

                if (cli == null)
                {
                    //Serialize parameters as a array with first element being the action
                    MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

                }
                else
                {
                    //Serialize parameters as a array with first element being the action
                    MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
                }
            }
        }
        else if (action == "updatePlayerHand")
        {
            //"updatePlayerHand" updates the hand of the player sent
            Character c = (Character)args[0];
            List<Card> cards = (List<Card>)args[1];
            List<ActionKind> l  = new List<ActionKind>();

            cards.OfType<ActionCard>().ToList().ForEach(t => l.Add(t.getKind()));

            var definition = new
            {
                eventName = action,
                player = c,
                cardsToAdd = l 
            };

            if (cli == null)
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

            }
            else
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
            }
        } else if (action == "updateGameStatus")
        {
            //"updateGameStatus" triggers either the Schemin or Stealin' phase
            var definition = new
            {
                eventName = action,
                statusIs = (bool)args[0],
            };
            if (cli == null)
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

            }
            else
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
            }
        }
        else if (action == "updateCurrentRound")
        {
            //"updateCurrentRound" visually updates the current round
            Round r = (Round)args[0];
            List<TurnType> l = new List<TurnType>();

            r.getTurns().ForEach(t => l.Add(t.getType()));

            var definition = new
            {
                eventName = action,
                isLastRound = r.getIsLastRound(),
                turns = l
            };
            if (cli == null)
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

            }
            else
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
            }
        }
        else if (action == "updateCurrentTurn")
        {
            //"updateCurrentTurn" visually updates the current turn
            var definition = new
            {
                eventName = action,
                currentTurn = (int)args[0]
            };

            if (cli == null)
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

            }
            else
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
            }

        } else if (action == "updateWaitingForInput")
        //"updateWaitingForInput" unlocks the UI for the current player
        //If the game status is Schemin', the turn menu is unlocked - expect the player to be able to play a card or draw cards
        //If the game status is Stealin', the board is unlocked
        {
            var definition = new
            {
                eventName = action,
                currentPlayer = (Character)args[0],
                waitingForInput = (bool)args[1],
            };

            if (cli == null)
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToAllClients(JsonConvert.SerializeObject(definition, settings));

            }
            else
            {
                //Serialize parameters as a array with first element being the action
                MyTcpListener.sendToClient(cli, JsonConvert.SerializeObject(definition, settings));
            }
        }
       
        List<System.Object> objectsToSerialize = new List<System.Object>();
        objectsToSerialize.Add(action);
        objectsToSerialize.AddRange(args);

       
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


