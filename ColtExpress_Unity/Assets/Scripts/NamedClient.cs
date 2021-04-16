using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using GameUnitSpace;
using System.Net;


public class NamedClient : MonoBehaviour
{
    public static Character c;

    private static TcpClient thisClient;
    private static NetworkStream stream;
    
    private static string buffer = "";
    private static string previousStateOfBuffer = "";
    private static bool previousStateOfBufferSaved = true;
    Boolean connected;

    public void connectToServer()
    {
        Debug.Log("Connecting to server...");

        //Open a connection to the server automatically upon uponing the game executable
        try
        {
            // Create a TcpClient; "serverIP" must be an IP running the corresponding server executable
            // // IPAddress serverAddr = IPAddress.Parse(serverIP);
            // Debug.Log("1");
            // IPEndPoint serverEndPoint = new IPEndPoint(serverAddr, port);
            // Debug.Log("2");
            // TcpClient thisClient = new TcpClient(serverEndPoint);
 
            thisClient = new TcpClient("52.152.132.200", 80);
            Debug.Log("3");
            //Obtain the corresponding stream
            stream = thisClient.GetStream();
            Debug.Log("4");

            // // Use socket to skip DNS lookup
            // Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(this.serverIP), this.port);
            // sock.Connect(ipep);
            // stream = new NetworkStream(sock);           
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("5");
            Debug.Log(e.Message);
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Debug.Log("6");
            Debug.Log(e.Message);

            Console.WriteLine("SocketException: {0}", e);
        }
        Debug.Log("7");

        connected = true;

    }



    void Update()
    {
        if (connected){
            //Listen for messages from the server
            getFromServer();
        }
    }

    public bool bufferIsEmpty()
    {
        return (buffer == "");
    }

    public void sendToServer(string message)
    {
        Debug.Log("[ClientToServer] Data transmitted: " + message);

        try
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.


            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }
    public static string getFromServer()
    {
        int i;
        string data = null;
        Byte[] bytes = new Byte[100000];

        //While there is data on the stream, add it to the buffer
        while (stream.DataAvailable)
        {
            i = stream.Read(bytes, 0, bytes.Length);
            Debug.Log("i :" + i);
            // Translate data bytes to a ASCII string.
            string message = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

            buffer += message;

            
        }
        Debug.Log("[ServerToClient] BUFFER: " + buffer);

        //If the buffer is not empty, continually extract the first event message and execute it
        while (!buffer.Equals("")) 
        {
            Regex splitBuffer = new Regex("\\{\"eventName\".*?\\{\"eventName\"(.*)", RegexOptions.IgnoreCase);

            Match match = splitBuffer.Match(buffer);

            if (match.Groups[0].Success)
            {
                string restOfBuffer = "{" + "\"eventName\"" + match.Groups[1].Value.ToString();

                Debug.Log("[ServerToClient] RESTOFBUFFER: " + restOfBuffer);

                int index = buffer.IndexOf(restOfBuffer);
                data = (index < 0)? buffer
                    : buffer.Remove(index, restOfBuffer.Length);
                buffer = restOfBuffer;

                Debug.Log("[ServerToClient] CURRENTMSG: " + data);
                ClientCommunicationAPIHandler.CommunicationAPIHandler.getMessageFromServer(data);
                //If the buffer has only one occurence of {"eventName", stop in case the rest of the message is broken off
                //On next getUpdate call, will be fine
                if (buffer.LastIndexOf("{" + "\"eventName\"") == buffer.IndexOf("{" + "\"eventName\"") && buffer != data)
                {
                    break;
                }
            }
            else
            {
                //Only one instance of {"eventName"
                //Save previous state of buffer
                //break to let the buffer get more data if needed
                if (previousStateOfBufferSaved)
                {
                    previousStateOfBuffer = buffer;
                    previousStateOfBufferSaved = false;
                    break;
                }
                else
                {

                    Debug.Log("[ServerToClient] BUFFER HAS BRKNMSG OR 1MSG: " + buffer);
                    //If the buffer on next call is eq to the previous state, it is one wellformed message
                    if (previousStateOfBuffer == buffer)
                    {
                        if (bracketsBalanced(buffer))
                        {
                            Debug.Log("[ServerToClient] HANDLER CALLED: " + buffer);
                            data = buffer;
                            buffer = "";
                            ClientCommunicationAPIHandler.CommunicationAPIHandler.getMessageFromServer(data);
                            previousStateOfBufferSaved = true;
                        }
                        
                    }
                    else
                    {
                        previousStateOfBufferSaved = true;
                    }

                }
                break;
                
            }
        }

        return data;

    }

    private static bool bracketsBalanced(string text)
    {
        Stack<char> s = new Stack<char>();

        foreach (char x in text)
        {
            if (x == '{')
            {
                s.Push(x);
            }
            else if (x == '}')
            {
                if (s.Count == 0)
                {
                    return false;
                }

                s.Pop();
            }
        }

        if (s.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }
}
