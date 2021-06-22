using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Framework.Service.Network
{
    public class TcpChannel : INetworkChannel
    {
        const int MAX_READ = 2048 * 1024;
        Socket client;
        MemoryStream stream;
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
        public Action<IAsyncResult> OnConnented { get; set; }

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

        public TcpChannel()
        {
            sendQueue = new Queue<byte[]>();
            recvQueue = new Queue<byte[]>();
            byteBuffer = new byte[MAX_READ];
            bytesPool = new BytesPool(MAX_READ);
            stream = new MemoryStream();
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
                var addressFamily = ipAddress.AddressFamily == AddressFamily.InterNetworkV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;
                client = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(ipAddress, port, OnConnected, client);
            }
            catch (Exception e)
            {
                Close();
                UnityEngine.Debug.LogErrorFormat("无法连接到服务器:{0}", e.ToString());
            }
        }

        byte[] buffer = new byte[1024];

        /// <summary>
        /// 连接成功的回调
        /// </summary>
        /// <param name="result"></param>
        void OnConnected(IAsyncResult result)
        {
            var state = (Socket)result.AsyncState;
            try
            {
                state.EndConnect(result);
                OnConnented?.Invoke(result);
                UnityEngine.Debug.Log("成功连接到服务器！！！");
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnRead, client);
                //client.BeginReceive(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position), SocketFlags.None, OnRead, client);
            }
            catch (Exception ex)
            {
                OnConnectionFailed?.Invoke();
                UnityEngine.Debug.LogErrorFormat("连接失败:{0}", ex.ToString());
                Close();
            }
        }

        void OnRead(IAsyncResult result)
        {
            var bytes = new byte[1024];
            var data = stream.GetBuffer();
            UnityEngine.Debug.Log(bytesToString(buffer));
            //int bytesRead = 0;
            //try
            //{
            //    lock (stream)
            //    {
            //        bytesRead = stream.EndRead(result);
            //    }
            //    if (bytesRead <= 0)
            //    {
            //        Close();
            //        UnityEngine.Debug.LogWarning("Connection was closed by the server: bytesRead < 1");
            //        return;
            //    }

            //    byte[] recv = new byte[1024];// bytesPool.Pop();
            //    Array.Copy(byteBuffer, recv, byteBuffer.Length);
            //    recvQueue.Enqueue(recv);

            //    lock (stream)
            //    {
            //        Array.Clear(byteBuffer, 0, byteBuffer.Length);
            //        stream.BeginRead(byteBuffer, 0, MAX_READ, OnRead, null);
            //        UnityEngine.Debug.Log(bytesToString(recv));
            //        UnityEngine.Debug.Log(Encoding.UTF8.GetString(recv));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    UnityEngine.Debug.LogErrorFormat("接收消息失败:{0}", ex.ToString());
            //    Close();
            //}
        }

        public static string bytesToString(byte[] bytes)
        {
            var result = "";
            foreach (var b in bytes)
            {
                result += b;
            }
            return result;
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
                UnityEngine.Debug.LogError("与服务器断开连接!!!");
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
            stream.BeginWrite(bytes, 0, bytes.Length, OnWrited, null);
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
            stream.Close();
            client.Close();
            OnClosed?.Invoke();
        }
    }
}