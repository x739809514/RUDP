using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class RUDPClient : MonoBehaviour
{ 
    private string ipAddress = "127.0.0.1";
    private int port = 5555;
    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private int nextSequenceNumber = 0;
    private int expectedSequenceNumber = 0;
    private Dictionary<int, byte[]> sendDataDic = new Dictionary<int, byte[]>();

   private void Start()
   {
       udpClient = new UdpClient();
       endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
       Debug.Log("Client is started...");
       
       udpClient.BeginReceive(ReceiveDataHandle, null);
       SendData("Hello, Server");
   }


#region UDP

    private void ReceiveDataHandle(IAsyncResult ar)
    {
        byte[] receiveData = udpClient.EndReceive(ar, ref endPoint);
        ReceiveData(receiveData);
    }
   
    private void SendPacketHandle(byte[] packet)
    {
        udpClient.Send(packet, packet.Length, endPoint);
       
        string message = System.Text.Encoding.UTF8.GetString(packet);
        Debug.Log("Send to Server: " + message);
    }

#endregion
   

#region RUDP

    private void SendData(string message)
    {
        byte[] sendData = System.Text.Encoding.UTF8.GetBytes(message);
        byte[] packet = AttachSequenceToPacket(sendData);
        SendPacketHandle(packet);
    }

    // attach sequence number to packet
    private byte[] AttachSequenceToPacket(byte[] data)
    {
        byte[] packet = new byte[data.Length + sizeof(int)];
        BitConverter.GetBytes(nextSequenceNumber).CopyTo(packet,0);
        data.CopyTo(packet,sizeof(int));
       
        sendDataDic.Add(nextSequenceNumber,packet);
        nextSequenceNumber++;
        return packet;
    }

    private void ReceiveData(byte[] packet)
    {
        int sequenceNumber = BitConverter.ToInt32(packet,0);
        if (sequenceNumber==expectedSequenceNumber)
        {
            byte[] data = new byte[packet.Length - sizeof(int)];
            Array.Copy(packet,sizeof(int),data,0,data.Length);
           
            // process receive data
            PrintData(data);

            expectedSequenceNumber++;
            while (sendDataDic.ContainsKey(expectedSequenceNumber))
            {
                byte[] outOrderData = new byte[packet.Length - sizeof(int)];
                Array.Copy(packet,sizeof(int),outOrderData,0,outOrderData.Length);
               
                // process receive data
                PrintData(outOrderData);

                sendDataDic.Remove(expectedSequenceNumber);
                expectedSequenceNumber++;
            }
        }
        else
        {
            sendDataDic.Add(expectedSequenceNumber,packet);
        }
    }

#endregion
   

   private void PrintData(byte[] data)
   {
       string message = System.Text.Encoding.UTF8.GetString(data);
       Debug.Log("Receive from Server: " + message);
   }
}
