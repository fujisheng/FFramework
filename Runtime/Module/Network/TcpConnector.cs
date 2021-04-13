using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Framework.Module.Network
{
    public class TcpConnector : IConnector
    {
        const int MAX_READ = 2048 * 1024;
        TcpClient client;
        NetworkStream networkStream;
        byte[] byteBuffer;
        Queue<byte[]> sendQueue;
        Queue<byte[]> recvQueue;
        bool writeCurrentComplete = true;
        ObjectPool<byte[]> bytesPool;

        /// <summary>
        /// 是否已经连接到服务器
        /// </summary>
        public bool IsConnected
        {
            get { return client != null && client.Connected; }
        }

        /// <summary>
        /// 连接成功的时候调用
        /// </summary>
        public Action OnConnented { get; set; }

        /// <summary>
        /// 连接失败的时候调用
        /// </summary>
        public Action OnConnectionFailed { get; set; }

        /// <summary>
        /// 关闭连接的时候调用
        /// </summary>
        public Action OnClosed { get; set; }

        /// <summary>
        /// 收到消息的时候调用 注意在调用完这个byte[]会被清空并且回收再利用
        /// </summary>
        public Action<byte[]> OnReceive { get; set; }

        public TcpConnector()
        {
            sendQueue = new Queue<byte[]>();
            byteBuffer = new byte[MAX_READ];
            bytesPool = new BytesPool(MAX_READ);
        }

        ~TcpConnector()
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
                IPAddress[] address = Dns.GetHostAddresses(ip);
                if (address.Length == 0)
                {
                    Debug.LogError("ip地址非法!!!");
                    return;
                }
                if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    client = new TcpClient(AddressFamily.InterNetworkV6);
                }
                else
                {
                    client = new TcpClient(AddressFamily.InterNetwork);
                }

                client.NoDelay = true;
                client.SendTimeout = 1000;
                client.ReceiveTimeout = 1000;
                client.BeginConnect(ip, port, OnConnected, null);
            }
            catch (Exception e)
            {
                Close();
                Debug.LogErrorFormat("无法连接到服务器:{0}", e.ToString());
            }
        }

        void OnConnected(IAsyncResult result)
        {
            TcpClient client = result.AsyncState as TcpClient;
            try
            {
                client.EndConnect(result);
                networkStream = client.GetStream();
                OnConnented?.Invoke();
                Debug.Log("成功连接到服务器！！！");
                networkStream.BeginRead(byteBuffer, 0, MAX_READ, OnRead, client);
            }
            catch (Exception ex)
            {
                OnConnectionFailed?.Invoke();
                Debug.LogErrorFormat("连接失败:{0}", ex.ToString());
                Close();
            }
        }

        void OnRead(IAsyncResult result)
        {
            int bytesRead = 0;
            try
            {
                lock (networkStream)
                {
                    bytesRead = networkStream.EndRead(result);
                }
                if (bytesRead <= 0)
                {
                    Close();
                    Debug.LogWarning("Connection was closed by the server: bytesRead < 1");
                    return;
                }

                byte[] recv = bytesPool.Pop();
                Array.Copy(byteBuffer, recv, byteBuffer.Length);
                recvQueue.Enqueue(recv);

                lock (networkStream)
                {
                    Array.Clear(byteBuffer, 0, byteBuffer.Length);
                    networkStream.BeginRead(byteBuffer, 0, MAX_READ, OnRead, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("接收消息失败:{0}", ex.ToString());
                Close();
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
                Debug.LogError("与服务器断开连接!!!");
                return;
            }

            sendQueue.Enqueue(bytes);
        }

        /// <summary>
        /// 更新方法
        /// </summary>
        public void OnUpdate()
        {
            if(sendQueue.Count > 0)
            {
                //防止消息过多的时候开启过多的线程去写入
                if (writeCurrentComplete == false)
                {
                    return;
                }

                WriteMessage(sendQueue.Dequeue());
            }

            if(recvQueue.Count > 0)
            {
                byte[] recv = recvQueue.Dequeue();
                OnReceive.Invoke(recv);
                Array.Clear(recv, 0, recv.Length);
                bytesPool.Push(recv);
            }
        }

        void WriteMessage(byte[] bytes)
        {
            writeCurrentComplete = false;
            networkStream.BeginWrite(bytes, 0, bytes.Length, OnWrited, null);
        }

        void OnWrited(IAsyncResult result)
        {
            writeCurrentComplete = true;
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
            sendQueue.Clear();
            recvQueue.Clear();
            bytesPool.Dispose();
            networkStream.Close();
            client.Close();
            OnClosed?.Invoke();
        }
    }
}