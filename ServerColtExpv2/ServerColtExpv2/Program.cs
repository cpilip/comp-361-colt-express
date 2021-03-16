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
using RoundSpace;

class MyTcpListener
{
    public static TcpClient currentClient;
    public static Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>();
    public static Dictionary<TcpClient, NetworkStream> clientStreams = new Dictionary<TcpClient, NetworkStream>();

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
                if (clients.Count == 1)
                {
                    haveAllConnections = true;
                }
            }

            Console.WriteLine("All clients successfully connected.");

            //Main game loop
            while (true)
            {

                Character c = JsonConvert.DeserializeObject<Character>(getFromClient());
                GameController.getInstance().chosenCharacter(c);
                break;

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

    public static void sendToClient(string data)
    {
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

        clientStreams.TryGetValue(currentClient, out NetworkStream streamToSendto);
        streamToSendto.Write(msg, 0, msg.Length);
        Console.WriteLine("Sent to Client: {0}", data);
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

        Console.WriteLine("Received {0} from {1}", data, fromClientatIP);

        return data;
    }

    
}