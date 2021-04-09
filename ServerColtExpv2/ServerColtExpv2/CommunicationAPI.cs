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
using HostageSpace;

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
            typeof(Shotgun),
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

        if (action == "updatePlayers")
        {
        //"updatePlayers" triggers creation of player profiles for every player in the list provided;
            List<Player> t = (List<Player>)args[0];

            foreach (Player n in t)
            {
                var definition = new
                {
                    eventName = "updatePlayer",
                    player = n.getBandit(),
                    numOfBullets = 6 - n.getNumOfBulletsShot()
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
        else if (action == "updateTrain")
        {
        //"updateTrain" triggers the initialization of the train
            List<TrainCar> t = (List<TrainCar>)args[0];

            int i = 0;
            foreach (TrainCar n in t)
            {
                var definition = new
                {
                    eventName = action,
                    indexofCar = i,
                    i_items = n.getInside().getUnits_Items(),
                    i_players = n.getInside().getUnits_Players(),
                    i_hasMarshal = n.getInside().hasMarshal(Marshal.getInstance()),
                    i_hasShotgun = n.getInside().hasShotgun(Shotgun.getInstance()),

                    r_items = n.getRoof().getUnits_Items(),
                    r_players = n.getRoof().getUnits_Players(),
                    r_hasMarshal = n.getRoof().hasMarshal(Marshal.getInstance()),
                    r_hasShotgun = n.getInside().hasShotgun(Shotgun.getInstance())
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
        }
        else if (action == "updatePlayerHand")
        {
        //"updatePlayerHand" updates the hand of the player sent
            var definition = new
            {
                eventName = action,
                player = (Character)args[0],
                cardsToAdd = (List<Card>)args[1]
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
        else if (action == "updateGameStatus")
        {
        //"updateGameStatus" triggers either the Schemin or Stealin' phase
            var definition = new
            {
                eventName = action,
                statusIs = (GameStatus)args[0],
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

        }
        else if (action == "updateWaitingForInput")
        //"updateWaitingForInput" unlocks the UI for the current player
        //If the game status is Schemin', the turn menu is unlocked - expect the player to be able to play a card or draw cards
        //If the game status is Stealin', the board is unlocked
        {
            var definition = new
            {
                eventName = action,
                currentPlayer = (Character)args[0],
                waitingForInput = (bool)args[1]
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
        else if (action == "addCards")
        //"addCards" adds cards for the current player
        {
            var definition = new
            {
                eventName = action,
                cardsToAdd = (List<Card>)args[0]
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
        else if (action == "updateCurrentPlayer")
        //"updateCurrentPlayer"  visually updates the current player
        {
            var definition = new
            {
                eventName = action,
                currentPlayer = (Character)args[0]
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
        else if (action == "decrementLoot")
        //"decrement" will decrement the visual icon in the player profile for the provided type of loot
        {
            var definition = new
            {
                eventName = action,
                player = (Character)args[0],
                loot = (GameItem)args[1]
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
        else if (action == "incrementLoot")
        //"decrement" will decrement the visual icon in the player profile for the provided type of loot
        {
            var definition = new
            {
                eventName = action,
                player = (Character)args[0],
                loot = (GameItem)args[1]
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
        else if (action == "decrementBullets")
        //"decrement" will decrement the visual icon in the player profile for the provided type of loot
        {
            var definition = new
            {
                eventName = action,
                player = (Character)args[0],
                numOfBullets = 6 - (int)args[1]
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
        else if (action == "decrementWhiskey")
        //"decrement" will decrement the visual icon in the player profile for the provided type of loot
        {
            var definition = new
            {
                eventName = action,
                player = (Character)args[0],
                whiskey = (WhiskeyKind)args[1]
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
        else if (action == "incrementWhiskey")
        //"increment" will increment the visual icon in the player profile for the provided type of loot
        {
            var definition = new
            {
                eventName = action,
                player = (Character)args[0],
                whiskey = (WhiskeyKind)args[1]
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
        else if (action == "updateHasAnotherAction")
        //"updateHasAnotherAction" will 
        {
            var definition = new
            {
                eventName = action,
                currentPlayer = (Character)args[0],
                hasAnotherAction = (bool)args[1],
                otherAction = (string)args[2]
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
        else if (action == "updateFirstPlayer")
        //"updateHasAnotherAction" will 
        {
            var definition = new
            {
                eventName = action,
                currentPlayer = (Character)args[0]
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
        else if (action == "updateTopCard")
        //"updateHasAnotherAction" will 
        {
            var definition = new
            {
                eventName = action,
                fromPlayer = (Character)args[0],
                card = (ActionKind)args[1]
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
        else if (action == "availableHostages")
        {
            List<HostageChar> l = new List<HostageChar>();
            ((List<Hostage>)args[0]).ForEach(h => l.Add(h.getHostageChar()));

            var definition = new
            {
                eventName = action,
                availableHostages = l
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
        else if (action == "updateHostageName")
        {
            var definition = new
            {
                eventName = action,
                player = (Character)args[0],
                hostage = (HostageChar)args[1]
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
        else if (action == "moveGameUnit")
        {
            Position p = (Position)args[1];

            var definition = new
            {
                eventName = action,
                gameUnit = (GameUnit)args[0],
                position = p,
                isInStageCoach = (p.getTrainCar() is StageCoach) ? true : false,
                trainCarIndex = (int)args[2]
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
        else if (action == "updateMovePositions" || action == "updatePunchPositions")
        {
            var definition = new
            {
                eventName = action,
                positions = (List<Position>)args[0],
                indices = (List<int>)args[1]
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
        else if (action == "updatePossTarget")
        {
            List<Character> l = new List<Character>();
            ((List<Player>)args[0]).ForEach(x => l.Add(x.getBandit()));

            var definition = new
            {
                eventName = action,
                targets = l
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
        else if (action == "updatePossTargetPunch")
        {
            List<Character> l = new List<Character>();
            ((List<Player>)args[0]).ForEach(x => l.Add(x.getBandit()));

            var definition = new
            {
                eventName = action,
                targets = l,
                shotgunCanBePunched = (bool)args[1]
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
        else if (action == "removeTopCard" || action == "highlightTopCard" || action == "updateSelectHostage" || action == "moveStageCoach")
        //"updateHasAnotherAction" will 
        {
            var definition = new
            {
                eventName = action,
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
        else
        {
            Console.WriteLine("Message " + action + " not implemented.");
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


