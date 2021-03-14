using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class NamedClient : MonoBehaviour
{

    public string server;
    public int port;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendMessageToServer(string message)
    {
        Debug.Log("Message sent");
      Connect(server, port, message);
        Debug.Log("Received");
    }

    void Connect(string server, int port, string message)
    {
        try
        {
    // Create a TcpClient.
    // Note, for this client to work you need to have a TcpServer
    // connected to the same address as specified by the server, port
    // combination.
    TcpClient client = new TcpClient(server, port);

    // Translate the passed message into ASCII and store it as a Byte array.
    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

    // Get a client stream for reading and writing.
   //  Stream stream = client.GetStream();

    NetworkStream stream = client.GetStream();

    // Send the message to the connected TcpServer.
    stream.Write(data, 0, data.Length);

    Console.WriteLine("Sent: {0}", message);

    // Receive the TcpServer.response.

    // Buffer to store the response bytes.
    data = new Byte[256];

    // String to store the response ASCII representation.
    String responseData = String.Empty;

    // Read the first batch of the TcpServer response bytes.
    Int32 bytes = stream.Read(data, 0, data.Length);
    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    Console.WriteLine("Received: {0}", responseData);
            Debug.Log(responseData);
            CommunicationAPIHandler.getMessageFromServer(responseData);

            // Close everything.
            stream.Close();
    client.Close();
  }
  catch (ArgumentNullException e)
  {
    Console.WriteLine("ArgumentNullException: {0}", e);
  }
  catch (SocketException e)
  {
    Console.WriteLine("SocketException: {0}", e);
  }

  Console.WriteLine("\n Press Enter to continue...");
  Console.Read();
}
}
