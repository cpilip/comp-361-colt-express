using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using GameUnitSpace;

public class NamedClient : MonoBehaviour
{
    public static Character c;

    private static TcpClient thisClient;
    private static NetworkStream stream;
    public string server;
    public int port;

    private static string buffer = "";

    void Start()
    {
        //Open a connection to the server automatically upon uponing the game executable
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            thisClient = new TcpClient(server, port);
            stream = thisClient.GetStream();
            

            //Byte[] data = System.Text.Encoding.ASCII.GetBytes("Hey");

            // Get a client stream for reading and writing.

            
            // Send the message to the connected TcpServer.
            //stream.Write(data, 0, data.Length);
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

    void Update()
    {
        getFromServer();
    }

    public void SendMessageToServer(string message)
    {
        Debug.Log("Message sent");
        TransmitMessage(server, port, message);
        Debug.Log("Received");
    }

    void TransmitMessage(string server, int port, string message)
    {
        try { 

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
        Byte[] bytes = new Byte[256];
        // Loop to receive all the data sent by the client.

        // Debug.Log("Getting from server.");

        
        while (stream.DataAvailable)
        {
            i = stream.Read(bytes, 0, bytes.Length);
            // Translate data bytes to a ASCII string.
            string message = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            //Debug.Log(buffer);

            buffer += message;

            Debug.Log("Received.");
        }

        while (!buffer.Equals("")) 
        {
            Regex splitBuffer = new Regex("\\{\"eventName\".*?\\{\"eventName\"(.*)", RegexOptions.IgnoreCase);

            if (splitBuffer.Match(buffer).Groups[0].Success)
            {
                string restOfBuffer = "{" + "\"eventName\"" + splitBuffer.Match(buffer).Groups[1].Value.ToString();

                //Debug.Log("REST" + restOfBuffer);

                int index = buffer.IndexOf(restOfBuffer);
                data = (index < 0)
                    ? buffer
                    : buffer.Remove(index, restOfBuffer.Length);
                buffer = restOfBuffer;

                //Debug.Log("CLEANED" + data);
            }
            else
            {
                data = buffer;
                buffer = "";
            }
            Debug.Log("DEBUG: " + buffer);
            CommunicationAPIHandler.getMessageFromServer(data);
        }
        


        // Debug.Log("No message.");

        return data;

    }
}
