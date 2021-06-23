using Framework.Collections;
using System;
using System.Net;
using System.Net.Sockets;

namespace Framework.Service.Network
{
    public class TcpChannel : INetworkChannel
    {
        const int maxRead = 1024;
        readonly Socket socket;
        readonly RingBuffer<byte> recvBuffer;
        readonly RingBuffer<byte> sendBuffer;
        readonly byte[] buffer; 

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
        public Action<byte[]> OnReceiveHandler { get; set; }

        public TcpChannel()
        {
            socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            recvBuffer = new RingBuffer<byte>(maxRead * 10);
            sendBuffer = new RingBuffer<byte>(maxRead * 10);
            buffer = new byte[maxRead];
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
                //var addressFamily = ipAddress.AddressFamily == AddressFamily.InterNetworkV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
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
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket);
                OnConnectionSuccessfulHandler?.Invoke(result);
            }
            catch (Exception ex)
            {
                Close();
                OnConnectionFailedHandler?.Invoke(ex.ToString());
            }
        }

        /// <summary>
        /// 收到消息的回调
        /// </summary>
        /// <param name="result"></param>
        void OnReceive(IAsyncResult result)
        {
            recvBuffer.Put(buffer);
            Array.Clear(buffer, 0, buffer.Length);
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

            sendBuffer.Put(bytes);
        }

        /// <summary>
        /// 更新方法
        /// </summary>
        public void OnUpdate()
        {
            //TODO 缓存这两个buffer
            if(sendBuffer.Size > 0)
            {
                var send = new byte[maxRead];
                sendBuffer.Get(maxRead, send);
                socket.BeginSend(send, 0, send.Length, SocketFlags.None, OnSend, socket);
            }

            if(recvBuffer.Size > 0)
            {
                var recv = new byte[maxRead];
                recvBuffer.Get(maxRead, recv);
                OnReceiveHandler?.Invoke(recv);
            }
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
            sendBuffer.Clear();
            Array.Clear(buffer, 0, buffer.Length);
            socket.Close();
        }
    }
}