
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;

    public class RUDPServer : MonoBehaviour
    {
        private Dictionary<int, byte[]> receiveDataDic = new Dictionary<int, byte[]>();
        private int nextSequenceNumber = 0;
        private UdpClient udpServer;
        private IPEndPoint remoteEndPoint;
        private int port = 5555;

        private void Start()
        {
            udpServer = new UdpClient();
            remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
            Debug.Log("Server is beginning, please wait...");

            udpServer.BeginReceive(ReceiveHandle, null);
        }

        private void ReceiveHandle(IAsyncResult ar)
        {
            byte[] receiveData = udpServer.EndReceive(ar, ref remoteEndPoint);
            ReceiveData(receiveData);

            // continue to receive...
            udpServer.BeginReceive(ReceiveHandle, null);
        }

        private void SendHandle(byte[] data, IPEndPoint endPoint)
        {
            udpServer.Send(data, data.Length, endPoint);
        }

        private void ReceiveData(byte[] packet)
        {
            int sequenceNumber = BitConverter.ToInt32(packet, 0);

            if (receiveDataDic.ContainsKey(sequenceNumber)==false)
            {
                receiveDataDic.Add(sequenceNumber, packet);

                while (receiveDataDic.ContainsKey(nextSequenceNumber))
                {
                    byte[] data = receiveDataDic[nextSequenceNumber];
                    
                    //process data
                    PrintData(data);

                    receiveDataDic.Remove(nextSequenceNumber);
                    nextSequenceNumber++;
                }
            }
            
            SendAcknowledge(sequenceNumber);
        }

        private void SendAcknowledge(int sequenceNumber)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(sequenceNumber.ToString());
            SendHandle(data,remoteEndPoint);
        }

        private void PrintData(byte[] data)
        {
            string message = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log("receive from Client: " + message);
        }
    }
