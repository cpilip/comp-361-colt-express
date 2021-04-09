using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameUnitSpace;
using Newtonsoft.Json;
using PositionSpace;
using CardSpace;
using RoundSpace;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using HostageSpace;


// Enter the listening loop.
// Loop here waiting for input from Lobby Service
// Obtain all the IPs from client directly !!!
// building Dictionary of IP's and stream
// Here is where each IP gets a TCP client

// while loop to wait for all the player to call chosenCharacters 
// When last player chose his character, initialization of the game state 

// Main while loop 

// for each player, wait for client response to play his turn (either playCard() or drawCards())
// endOfTurn() is called and current player is changed to the next one. 
// repeat for all turns of the round, move to Stealin phase 

// call readyForNextMove() 
// for each card is the playedCard pile, call corresponding client for neccessary information
// send game state to all clients, endOfCard() is called
// repeat until there are no cards in the pile
// if it is the last round, calculateGameScore() and exit the loop. 

// End of Game, send all clients to GameScore scene 

class MyTcpListener
{
    public static TcpClient currentClient;
    public static Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>();
    public static Dictionary<TcpClient, NetworkStream> clientStreams = new Dictionary<TcpClient, NetworkStream>();
    public static Dictionary<Player, TcpClient> players = new Dictionary<Player, TcpClient>();
    public static Byte[] bytes = new Byte[256];

    //Lobby Service starts Server APPLICATION or server APPLICATION is started and waiting for input from Lobby Service
    public static void Main()
    {
        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            String data = null;

            bool haveAllConnections = false;

            // Enter the listening loop for all clients to connect
            Console.WriteLine("Waiting for connections... ");


            while (haveAllConnections == false)
            {
                    //Open a stream for each client
                    TcpClient client = server.AcceptTcpClient();

                    IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                    NetworkStream currentClientStream = client.GetStream();

                    //Add each client mapped to IP address
                    clients.Add(client, "" + remoteIpEndPoint.Address);
                    clientStreams.Add(client, currentClientStream);

                    currentClient = client;


                    //Verify against lobby service
                    if (clients.Count == 3)
                    {
                        haveAllConnections = true;
                    }
            }

            Console.WriteLine("All clients successfully connected.");

            //MAIN GAME STARTS HERE

            // get permanent instance of GameController
            GameController aController = GameController.getInstance();

            // Listen to all players for their character selection
            foreach (TcpClient cli in clientStreams.Keys)
            {
                Character c = JsonConvert.DeserializeObject<Character>(getFromClient(cli));
                currentClient = cli;
                aController.chosenCharacter(c);

                Player p = aController.getPlayerByCharacter(c);
                //players.Add(p, cli);
            }

            

            while (!aController.getEndOfGame())
            {
                // Wait for first move of first player
                string res = getFromClient(players[aController.getCurrentPlayer()]);
                // Need to parse res and call the right GameController method.
                JObject o = JObject.Parse(res);
                string eventName = o.SelectToken("eventName").ToString();

                if (eventName.Equals("RobMessage"))
                {
                    // Get item
                    ItemType type = o.SelectToken("item").ToObject<ItemType>();
                    GameItem it = aController.getItemfromTypePosition(type);

                    aController.chosenLoot(it);
                }
                else if (eventName.Equals("ShootMessage"))
                {
                    // Get player
                    Character ch = o.SelectToken("target").ToObject<Character>();
                    Player pl = aController.getPlayerByCharacter(ch);

                    aController.chosenShootTarget(pl);
                }
                else if (eventName.Equals("PunchMessage"))
                {
                    // Get character
                    Character ch = o.SelectToken("target").ToObject<Character>();
                    Player pl = aController.getPlayerByCharacter(ch);

                    // Get item
                    ItemType type = o.SelectToken("item").ToObject<ItemType>();
                    GameItem it = aController.getItemfromTypePossession(type);

                    // Get position
                    int index = Int32.Parse(o.SelectToken("index").ToString());
                    Boolean inside = o.SelectToken("inside").ToObject<Boolean>();// ???????????
                    Position pos = aController.getPositionByIndex(index, inside);

                    aController.chosenPunchTarget(pl, it, pos);
                }
                else if (eventName.Equals("MoveMessage"))
                {
                    // Get position
                    int index = Int32.Parse(o.SelectToken("index").ToString());
                    Boolean inside = o.SelectToken("inside").ToObject<Boolean>();// ???????????
                    Position pos = aController.getPositionByIndex(index, inside);

                    aController.chosenPosition(pos);
                }
                else if (eventName.Equals("CardMessage"))
                {
                    // Get index; -1 if player timed out
                    int index = Int32.Parse(o.SelectToken("index").ToString());
                    if (index != -1)
                    {
                        ActionCard crd = aController.getCardByIndex(index);
                        aController.playActionCard(crd);
                    } 
                    else
                    {
                        //Player timed out
                        aController.playActionCard(null);
                    }
                }
                else if (eventName.Equals("DrawMessage"))
                {
                    aController.drawCards();
                }
                else if (eventName.Equals("WhiskeyMessage"))
                {
                    // Get whiskey kind
                    try
                    {
                        WhiskeyKind whiskey = o.SelectToken("usedWhiskey").ToObject<WhiskeyKind>();
                        aController.useWhiskey(whiskey);
                    } 
                    catch (Exception e)
                    {
                        //No usedWhiskey property, meaning player timed out on choosing a whiskey
                        //Pass null instead
                        WhiskeyKind? whiskey = null;
                        aController.useWhiskey(whiskey);
                    }
                } 
                else if (eventName.Equals("HostageMessage"))
                {
                    HostageChar hostage = o.SelectToken("chosenHostage").ToObject<HostageChar>();
                    aController.chosenHostage(hostage);

                }
            }

        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            //Close all connections
            var flattenListOfClients = clients.Keys.ToList();
            flattenListOfClients.ForEach(c => c.Close());

            //Stop listening for new clients.
            server.Stop();


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }

    public static TcpClient getClientByPlayer(Player p)
    {
        return players[p];
    }

    public static void addPlayerWithClient(Player p)
    {
        players.Add(p, currentClient);
    }

    public static void sendToClient(string data)
    {
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

        clientStreams.TryGetValue(currentClient, out NetworkStream streamToSendto);
        streamToSendto.Write(msg, 0, msg.Length);
        Console.WriteLine("Sent to Client: {0}", data);
    }

    // Send a message to a specific client
    public static void sendToClient(TcpClient cli, string data)
    {
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

        clientStreams.TryGetValue(cli, out NetworkStream streamToSendto);
        streamToSendto.Write(msg, 0, msg.Length);
        Console.WriteLine("Sent to Client: {0}", data);
    }

    // Send a message to all clients
    public static void sendToAllClients(string data)
    {
        foreach (TcpClient cli in clientStreams.Keys)
        {
            sendToClient(cli, data);
        }
    }

    public static string getFromClient(TcpClient toReadFrom)
    {
        clientStreams.TryGetValue(toReadFrom, out NetworkStream streamToReadFrom);

        int i;
        string data = null;
        // Loop to receive all the data sent by the client.
        while (streamToReadFrom.DataAvailable == false)
        {

        }

        //i = number of bytes read
        do
        {
            i = streamToReadFrom.Read(bytes, 0, bytes.Length);
            // Translate data bytes to a ASCII string.
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);


        } while (streamToReadFrom.DataAvailable);

        clients.TryGetValue(currentClient, out string fromClientatIP);

        Console.WriteLine("Received {0} from {1}", data, "client");

        // Console.WriteLine("Received {0} from {1}", data, fromClientatIP);

        return data;
    }
    public static string getFromClient()
    {
        clientStreams.TryGetValue(currentClient, out NetworkStream streamToReadFrom);

        int i;
        string data = null;
        // Loop to receive all the data sent by the client.
        while (streamToReadFrom.DataAvailable == false)
        {

        }

        //i = number of bytes read
        do
        {
            i = streamToReadFrom.Read(bytes, 0, bytes.Length);
            // Translate data bytes to a ASCII string.
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);


        } while (streamToReadFrom.DataAvailable);

        clients.TryGetValue(currentClient, out string fromClientatIP);

        Console.WriteLine("Received {0} from {1}", data, "client");
        // Console.WriteLine("Received {0} from {1}", data, fromClientatIP);

        return data;
    }


}