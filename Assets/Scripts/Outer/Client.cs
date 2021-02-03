using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

namespace CT.Net
{
    public class Client : MonoBehaviour
    {
        public static Client instance;
        public static int dataBufferSize = 4096;

        const string localHost = "127.0.0.1";
        public string ip = localHost;
        public int port = 26950;
        public int id = 0;

        public TCP tcp;
        public UDP udp;

        bool isConnected = false;

        delegate void PacketHandler(Packet packet);
        static Dictionary<int, PacketHandler> packetHandlers;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            tcp = new TCP();
            udp = new UDP();
        }

        void OnApplicationQuit()
        {
            Disconnect();
        }

        public void ConnectClientToServer()
        {
            InitClientData();
            tcp.Connect();
            if (tcp.socket != null && udp.socket != null)
                isConnected = true;
        }

        public class TCP
        {
            public TcpClient socket;

            NetworkStream stream;
            Packet receiveData;
            byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            }

            void ConnectCallback(IAsyncResult result)
            {
                socket.EndConnect(result);

                if (!socket.Connected) return;

                stream = socket.GetStream();

                receiveData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"Error sending data: {e}");
                }
            }

            void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int len = stream.EndRead(result);
                    if (len <= 0)
                    {
                        instance.Disconnect();
                        return;
                    }

                    var data = new byte[len];
                    Array.Copy(receiveBuffer, data, len);

                    receiveData.Reset(HandleData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch
                {
                    Disconnect();
                }
            }

            bool HandleData(byte[] data)
            {
                int length = 0;

                receiveData.SetBytes(data);

                if (receiveData.UnreadLength() >= 4)
                {
                    length = receiveData.ReadInt();
                    if (length <= 0) return true;
                }

                while (length > 0 && length <= receiveData.UnreadLength())
                {
                    var packetBytes = receiveData.ReadBytes(length);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (var packet = new Packet(packetBytes))
                        {
                            int packetID = packet.ReadInt();
                            packetHandlers[packetID](packet);
                        }
                    });

                    length = 0;
                    if (receiveData.UnreadLength() >= 4)
                    {
                        length = receiveData.ReadInt();
                        if (length <= 0) return true;
                    }
                }

                if (length <= 1) return true;

                return false;
            }

            void Disconnect()
            {
                instance.Disconnect();

                stream = null;
                receiveData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
            }

            public void Connect(int localPort)
            {
                socket = new UdpClient(localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (var packet = new Packet()) SendData(packet);
            }

            public void SendData(Packet packet)
            {
                try
                {
                    packet.InsertInt(instance.id);
                    if (socket != null)
                    {
                        socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"Error sending data to server via UDP: {e}");
                }
            }

            void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    var data = socket.EndReceive(result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4)
                    {
                        instance.Disconnect();
                        return;
                    }

                    HandleData(data);
                }
                catch
                {
                    Disconnect();
                }
            }

            void HandleData(byte[] data)
            {
                using (var packet = new Packet(data))
                {
                    int packetLength = packet.ReadInt();
                    data = packet.ReadBytes(packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (var packet = new Packet(data))
                    {
                        int packetID = packet.ReadInt();
                        packetHandlers[packetID](packet);
                    }
                });
            }

            void Disconnect()
            {
                instance.Disconnect();

                endPoint = null;
                socket = null;
            }
        }

        void InitClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPackets.welcomeFromServer, ClientHandle.Welcome },
                { (int)ServerPackets.registerConfirm, ClientHandle.RegisterConfirm },
                { (int)ServerPackets.loadPlayer, ClientHandle.LoadPlayer },
                { (int)ServerPackets.buildingAdded, ClientHandle.BuildingAdded },
                { (int)ServerPackets.giveFewRandomBases, ClientHandle.GetFewRandomTargetBases },
                { (int)ServerPackets.giveAttackableBaseData, ClientHandle.ReceiveAttackableBaseData },
                { (int)ServerPackets.updateResources, ClientHandle.UpdateResources },
                { (int)ServerPackets.updateGems, ClientHandle.UpdateGems },
                { (int)ServerPackets.giveUpdatedPlayerData, ClientHandle.ReceiveUpdatedPlayerData }
            };
            //Debug.Log("Initialized!");
        }

        void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();

                Debug.Log("Disconnected from server.");
            }
        }
    }
}