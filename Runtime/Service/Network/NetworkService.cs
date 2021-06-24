using System;

namespace Framework.Service.Network
{
    internal sealed class NetworkService : Service, INetworkService
    {
        INetworkChannel channel;
        IPackager packager;
        IEncryptor encryptor;
        ISerializer serializer;

        public Action<IAsyncResult> OnConnectionSuccessfulHandler { get; set; }
        public Action<string> OnConnectionFailedHandler { get; set; }
        public Action OnClosedHandler { get; set; }
        public Action<IPacket> OnReceiveHandler { get; set; }


        /// <summary>
        /// 设置消息打包器
        /// </summary>
        /// <param name="converter"></param>
        public void SetPackager(IPackager packager)
        {
            this.packager = packager;
        }

        /// <summary>
        /// 设置网络频道
        /// </summary>
        /// <param name="channel"></param>
        public void SetNetworkChannel(INetworkChannel channel)
        {
            this.channel = channel;
            this.channel.OnConnectionSuccessfulHandler += OnConnectionSuccessfulHandler;
            this.channel.OnConnectionFailedHandler += OnConnectionFailedHandler;
            this.channel.OnConnectionSuccessfulHandler += OnConnectionSuccessful;
            this.channel.OnConnectionFailedHandler += OnConnectionFailed;

            this.channel.OnReceiveHandler += OnReceive;
        }

        /// <summary>
        /// 设置加密器
        /// </summary>
        /// <param name="encryptor"></param>
        public void SetEncryptor(IEncryptor encryptor)
        {
            this.encryptor = encryptor;
        }

        /// <summary>
        /// 设置序列化器
        /// </summary>
        /// <param name="serializer"></param>
        public void SetSerializer(ISerializer serializer)
        {
            this.serializer = serializer;
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port)
        {
            if(packager == null)
            {
                throw new Exception("Packager is null, you need set packager first");
            }

            if(channel == null)
            {
                throw new Exception("Packager is null, you need set network channel first");
            }

            channel.Connect(ip, port);
        }

        void OnConnectionSuccessful(IAsyncResult result)
        {
            UnityEngine.Debug.Log("Connected");
        }

        void OnConnectionFailed(string ex)
        {
            UnityEngine.Debug.LogError($"Connect Failed:{ex}");
        }

        /// <summary>
        /// 发送一个消息
        /// </summary>
        /// <param name="packet"></param>
        public void Send(IPacket packet)
        {
            if(channel == null || !channel.IsConnected)
            {
                UnityEngine.Debug.LogError("Connector为空 或者没有连接到服务器！！！");
                return;
            }

            byte[] bytes = packager.Unpack(packet);
            Send(bytes);
        }

        /// <summary>
        /// 通过一个数据类型发送一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void Send<T>(ushort id, T data)
        {
            var bytes = serializer.Serialize<T>(data);
            var bcc = PacketHead.CountBCC(bytes, 0, bytes.Length);
            //UnityEngine.Debug.Log($"sendMsg bcc:{bcc}");
            var encryptBytes = encryptor == null ? bytes : encryptor.Encrypt(bytes, 0, bytes.Length);
            bytes = packager.Unpack(id, encryptBytes, bcc);
            Send(bytes);
        }

        /// <summary>
        /// 直接发送一个byte数组
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                UnityEngine.Debug.LogWarning("要发送的消息为空!!!");
                return;
            }

            channel.Send(bytes);
        }

        public void AddListener<T>(int id, Action<object> listener)
        {

        }

        /// <summary>
        /// 当收到消息的时候
        /// </summary>
        /// <param name="bytes">收到的bytes</param>
        void OnReceive(IPacket packet)
        {
            //IPacket packet = packager.Pack(bytes);
            var decryptBytes = encryptor == null ? packet.Body : encryptor.Decrypt(packet.Body, 0, packet.Body.Length);
            packet.Body = decryptBytes;
            UnityEngine.Debug.Log($"receiveBytes:{packet.Body.Length}");
            OnReceiveHandler?.Invoke(packet);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            channel?.Close();
            OnClosedHandler?.Invoke();
        }

        internal override void OnUpdate()
        {
            channel?.OnUpdate();
        }

        internal override void OnTearDown()
        {
            Close();
        }
    }
}