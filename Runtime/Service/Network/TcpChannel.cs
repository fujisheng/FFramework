using Framework.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Service.Network
{
    public class TcpChannel : INetworkChannel
    {
        readonly Socket socket;

        /// <summary>
        /// 接收的包体最大大小
        /// </summary>
        const int sendPacketMaxSize = 1024 * 1024;

        /// <summary>
        /// 接收buffer的容量
        /// </summary>
        const int recvBufferCapacity = sendPacketMaxSize * 8;

        readonly RingBuffer<byte> recvBuffer;
        readonly Thread recvThread;
        bool recvThreadCancle = false;
        readonly byte[] buffer;
        IPacket currentPacket;

        /// <summary>
        /// 是否已经连接到服务器
        /// </summary>
        public bool IsConnected
        {
            get { return socket != null && socket.Connected; }
        }

        /// <summary>
        /// 连接成功的时候调用
        /// </summary>
        public Action<IAsyncResult> OnConnectionSuccessfulHandler { get; set; }

        /// <summary>
        /// 连接失败的时候调用
        /// </summary>
        public Action<string> OnConnectionFailedHandler { get; set; }

        /// <summary>
        /// 收到消息的时候调用
        /// </summary>
        public Action<IPacket> OnReceiveHandler { get; set; }

        public TcpChannel()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            recvBuffer = new RingBuffer<byte>(recvBufferCapacity);
            buffer = new byte[sendPacketMaxSize];
            recvThread = new Thread(ReceiveMessage);
            recvThread.IsBackground = true;
        }

        ~TcpChannel()
        {
            Close();
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口号</param>
        public void Connect(string ip, int port)
        {
            try
            {
                var ipAddress = IPAddress.Parse(ip);
                socket.BeginConnect(ipAddress, port, OnConnectionSuccessful, socket);
            }
            catch (Exception e)
            {
                OnConnectionFailedHandler?.Invoke(e.ToString());
                Close();
            }
        }

        /// <summary>
        /// 连接成功的回调
        /// </summary>
        /// <param name="result"></param>
        void OnConnectionSuccessful(IAsyncResult result)
        {
            var state = (Socket)result.AsyncState;
            try
            {
                state.EndConnect(result);
                recvThread.Start();
                OnConnectionSuccessfulHandler?.Invoke(result);
            }
            catch (Exception ex)
            {
                Close();
                OnConnectionFailedHandler?.Invoke(ex.ToString());
            }
        }

        void ReceiveMessage()
        {
            while (true)
            {
                if(recvThreadCancle == true)
                {
                    return;
                }

                try
                {
                    int size = socket.Receive(buffer);
                    if (size <= 0)
                    {
                        continue;
                    }

                    for(int i = 0; i < size; i++)
                    {
                        recvBuffer.Write(buffer[i]);
                    }

                    Array.Clear(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="bytes">直接就发过去了 不会做任何处理</param>
        public void Send(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            if (!IsConnected)
            {
                return;
            }
            socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, OnSend, socket);
        }

        /// <summary>
        /// 更新方法
        /// </summary>
        public void OnUpdate()
        {
            if (recvBuffer.Size > 0)
            {
                ProcessReceive();
            }
        }

        /// <summary>
        /// 处理接收消息
        /// </summary>
        void ProcessReceive()
        {
            if(currentPacket == null && recvBuffer.Size > 8)
            {
                //先读包头 
                var headBytes = new byte[8];
                recvBuffer.Read(8, headBytes);
                var head = DefaultPackager.ReadHead(headBytes);
                var packet = new Packet();
                packet.Head = head;
                currentPacket = packet;
            }

            if(currentPacket.Head.length < recvBuffer.Size)
            {
                return;
            }

            var data = new byte[currentPacket.Head.length];
            recvBuffer.Read(currentPacket.Head.length, data);
            currentPacket.Body = data;
            OnReceiveHandler?.Invoke(currentPacket);
            currentPacket = null;
        }

        /// <summary>
        /// 发送消息成功
        /// </summary>
        /// <param name="result"></param>
        void OnSend(IAsyncResult result)
        {
            
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (!IsConnected)
            {
                return;
            }
            recvBuffer.Clear();
            Array.Clear(buffer, 0, buffer.Length);
            socket.Close();
            recvThreadCancle = true;
        }
    }
}