using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using RoundSpace;

class MyTcpListener
{
    public static NetworkStream currentClientStream;


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

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;


            GameController aGameController = GameController.getInstance();

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


            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");


                IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine("Connection from: " + remoteIpEndPoint.Address);

                data = null;

                // Get a stream object for reading and writing
                currentClientStream = client.GetStream();

                /*int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0} from {1}", data, IPAddress.Parse(((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString()));

                    // Process the data sent by the client.
                    //data = data.ToUpper();

                    
                    // Send a message/response to the client
                    data = CommunicationAPI.sendMessageToClient("testJSON", new Turn(TurnType.Standard));
                    sendToClient(stream, data);
                    //data = "Your message was received, Client: " + IPAddress.Parse(((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString());

                } */

                //data = CommunicationAPI.sendMessageToClient("testJSON", new Turn(TurnType.Standard));
                //sendToClient(data);



                // Shutdown and end connection
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }

    public static void sendToClient(string data)
    {
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

        currentClientStream.Write(msg, 0, msg.Length);
        Console.WriteLine("Sent to Client: {0}", data);
    }

    public static void receiveFromClient(string data){




    }


}