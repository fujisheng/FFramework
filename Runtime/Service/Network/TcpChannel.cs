using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Framework.Service.Network
{
    public class TcpChannel : INetworkChannel
    {
        Socket socket;

        /// <summary>
        /// 接收的包体最大大小
        /// </summary>
        const int sendPacketMaxSize = 1024 * 1024;

        /// <summary>
        /// 接收buffer的容量
        /// </summary>
        const int recvBufferCapacity = sendPacketMaxSize * 8;

        readonly BytesBuffer recvBuffer;
        readonly byte[] buffer;
        INetworkPacket currentPacket;

        readonly Thread recvThread;
        bool beginReceive = false;
        bool recvThreadStarted = false;

        bool isConnected = false;

        /// <summary>
        /// 是否已经连接到服务器
        /// </summary>
        public bool IsConnected
        {
            get { return socket != null && socket.Connected && isConnected; }
            private set { isConnected = value; }
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
        public Action<INetworkPacket> OnReceiveHandler { get; set; }

        /// <summary>
        /// 关闭连接的时候调用
        /// </summary>
        public Action OnCloseHandler { get; set; }

        public TcpChannel()
        {
            recvBuffer = new BytesBuffer(recvBufferCapacity);
            buffer = new byte[sendPacketMaxSize];

            recvThread = new Thread(ReceiveMessage)
            {
                IsBackground = true
            };
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
            if(socket != null)
            {
                Close();
            }

            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                var ipAddress = IPAddress.Parse(ip);
                socket.BeginConnect(ipAddress, port, OnConnectionSuccessful, socket);
            }
            catch (Exception e)
            {
                Close();
                OnConnectionFailedHandler?.Invoke(e.ToString());
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
                beginReceive = true;

                if (!recvThreadStarted)
                {
                    recvThread.Start();
                    recvThreadStarted = true;
                }

                OnConnectionSuccessfulHandler?.Invoke(result);
            }
            catch (Exception ex)
            {
                Close();
                OnConnectionFailedHandler?.Invoke(ex.ToString());
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        void ReceiveMessage()
        {
            while (true)
            {
                if(!beginReceive)
                {
                    continue;
                }

                try
                {
                    int size = socket.Receive(buffer);

                    if (size <= 0)
                    {
                        continue;
                    }

                    recvBuffer.Write(buffer, 0, size);
                    Array.Clear(buffer, 0, size);
                }
                catch
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

            try
            {
                socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, OnSend, socket);
            }
            catch
            {
                Close();
            }
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
            if(currentPacket == null && recvBuffer.Size >= PacketHead.HeadLength)
            {
                unsafe
                {
                    //先读包头
                    var headBytes = stackalloc byte[PacketHead.HeadLength];
                    recvBuffer.Read(headBytes, PacketHead.HeadLength);
                    var head = new PacketHead(headBytes, PacketHead.HeadLength);
                    var packet = new Packet(head);
                    currentPacket = packet;
                }
            }

            //判断当前buffer中的数据长度够不够包头的长度 不够就等待 够就读出来
            if(currentPacket.Head.length > recvBuffer.Size)
            {
                return;
            }

            //TODO 这个地方可以改成pool形式
            var data = new byte[currentPacket.Head.length];
            recvBuffer.Read(data, currentPacket.Head.length);
            currentPacket.WriteData(data);
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
            IsConnected = false;
            recvBuffer.Clear();
            Array.Clear(buffer, 0, buffer.Length);
            beginReceive = false;
            socket?.Close();
            socket = null;
            OnCloseHandler?.Invoke();
        }
    }
}