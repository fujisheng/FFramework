using Framework.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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

        Queue<byte[]> recvQueue;
        readonly RingBuffer<byte> recvBuffer;
        byte[] buffer;

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
            recvBuffer = new RingBuffer<byte>(recvBufferCapacity);
            buffer = new byte[sendPacketMaxSize];
            recvQueue = new Queue<byte[]>();
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
            try
            {
                var ts = result as Socket;
                
                //Array.Clear(buffer, 0, buffer.Length);

                //var bytesReceived = ts.EndReceive(result);
                //if(bytesReceived <= 0)
                //{
                //    Close();
                //    return;
                //}

                UnityEngine.Debug.Log("OnReceive");
                var data = new byte[sendPacketMaxSize];
                Array.Copy(buffer, data, buffer.Length);
                Array.Clear(buffer, 0, buffer.Length);
                //recvBuffer.Write(data);
                recvQueue.Enqueue(data);
                //result.AsyncWaitHandle.Close();
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, socket);
            }
            catch(Exception ex)
            {

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
            if(recvQueue.Count > 0)
            {
                var data = recvQueue.Dequeue();
                OnReceiveHandler?.Invoke(data);
            }
            //if(recvBuffer.Size > 0)
            //{
            //    var recv = new byte[sendPacketMaxSize];
            //    recvBuffer.Read(sendPacketMaxSize, recv);
            //    OnReceiveHandler?.Invoke(recv);
            //}
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
        }
    }
}