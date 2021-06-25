using FInject;
using System;

namespace Framework.Service.Network
{
    internal sealed class NetworkService : Service, INetworkService
    {
        INetworkChannel channel;

        //必须的Helper
        INetworkPackageHelper packageHelper;
        INetworkSerializeHelper serializeHelper;
        INetworkBCCHelper bccHelper;

        //可选的Helper
        INetworkEncryptHelper encryptHelper;
        INetworkCompressHelper compressHelper;

        public Action<IAsyncResult> OnConnectionSuccessfulHandler { get; set; }
        public Action<string> OnConnectionFailedHandler { get; set; }
        public Action OnClosedHandler { get; set; }
        public Action<INetworkPacket> OnReceiveHandler { get; set; }

        /// <summary>
        /// 设置网络频道
        /// </summary>
        /// <param name="channel"></param>
        [Inject]
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
        /// 设置消息打包器
        /// </summary>
        /// <param name="converter"></param>
        [Inject]
        public void SetNetworkPackageHelper(INetworkPackageHelper packageHelper)
        {
            this.packageHelper = packageHelper;
        }

        /// <summary>
        /// 设置加密器
        /// </summary>
        /// <param name="encryptHelper"></param>
        [Inject]
        public void SetNetworkEncryptHelper(INetworkEncryptHelper encryptHelper)
        {
            this.encryptHelper = encryptHelper;
        }

        /// <summary>
        /// 设置序列化器
        /// </summary>
        /// <param name="serializeHelper"></param>
        [Inject]
        public void SetNetworkSerializeHelper(INetworkSerializeHelper serializeHelper)
        {
            this.serializeHelper = serializeHelper;
        }

        /// <summary>
        /// 设置bcc校验方法
        /// </summary>
        /// <param name="bccHelper"></param>
        [Inject]
        public void SetNetworkBCCHelper(INetworkBCCHelper bccHelper)
        {
            this.bccHelper = bccHelper;
        }

        /// <summary>
        /// 设置压缩方法
        /// </summary>
        /// <param name="compressHelper"></param>
        [Inject]
        public void SetNetworkCompressHelper(INetworkCompressHelper compressHelper)
        {
            this.compressHelper = compressHelper;
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port)
        {
            if(channel == null)
            {
                throw new Exception("Channel is null, you need call SetNetworkChannel first");
            }

            if (bccHelper == null)
            {
                throw new Exception("NetworkBCCHelper is null, you need call SetNetworkBCCHelper first");
            }

            if (packageHelper == null)
            {
                throw new Exception("NetworkPackageHelper is null, you need call SetNetworkPackageHelper first");
            }

            if(serializeHelper == null)
            {
                throw new Exception("NetworkSerializeHelper is null, you need call SetNetworkSerializeHelper first");
            }

            channel.Connect(ip, port);
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        /// <param name="result"></param>
        void OnConnectionSuccessful(IAsyncResult result)
        {
            UnityEngine.Debug.Log("Connected");
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        /// <param name="ex"></param>
        void OnConnectionFailed(string ex)
        {
            UnityEngine.Debug.LogError($"Connect Failed:{ex}");
        }

        /// <summary>
        /// 发送一个包
        /// </summary>
        /// <param name="packet">包</param>
        public void Send(INetworkPacket packet)
        {
            if (channel == null || !channel.IsConnected)
            {
                UnityEngine.Debug.LogError("Connector为空 或者没有连接到服务器！！！");
                return;
            }

            byte[] bytes = packageHelper.Unpack(packet);
            InternalSend(bytes);
        }

        /// <summary>
        /// 发送一个对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="id">消息id</param>
        /// <param name="data">数据</param>
        /// <param name="flag">标记</param>
        public void Send<T>(ushort id, T data, PacketFlag flag = PacketFlag.Encrypt)
        {
            var bytes = serializeHelper.Serialize<T>(data);
            Send(id, bytes, flag);
        }

        /// <summary>
        /// 发送一个数据bytes
        /// </summary>
        /// <param name="id">消息id</param>
        /// <param name="bytes">bytes</param>
        /// <param name="flag">标记</param>
        public void Send(ushort id, byte[] bytes, PacketFlag flag = PacketFlag.Encrypt)
        {
            var bcc = bccHelper.Calculation(bytes, 0, bytes.Length);
            var resultBytes = bytes;
            if (flag.HasFlag(PacketFlag.Encrypt))
            {
                if (encryptHelper == null)
                {
                    throw new Exception("Network Packet has Encrypt flag, but NetworkEncryptHelper is null, please call SetNetworkEncyptHelper first");
                }

                resultBytes = encryptHelper.Encrypt(bytes, 0, bytes.Length);
            }

            bytes = packageHelper.Unpack(id, resultBytes, bcc);
            InternalSend(bytes);
        }

        /// <summary>
        /// 直接发送一个byte数组
        /// </summary>
        /// <param name="bytes">bytes</param>
        void InternalSend(byte[] bytes)
        {
            if (!channel.IsConnected)
            {
                UnityEngine.Debug.LogFormat("还没有连接到服务器");
                return;
            }

            if (bytes == null || bytes.Length == 0)
            {
                UnityEngine.Debug.LogWarning("要发送的消息为空!!!");
                return;
            }

            channel.Send(bytes);
        }

        /// <summary>
        /// 当收到消息的时候
        /// </summary>
        /// <param name="packet">收到的包</param>
        void OnReceive(INetworkPacket packet)
        {
            var flags = (PacketFlag)packet.Head.flags;
            var resultBytes = packet.Data;

            //如有有加密标记 则执行解密行为
            if (flags.HasFlag(PacketFlag.Encrypt))
            {
                if(encryptHelper == null)
                {
                    throw new Exception("Network Packet has Encrypt flag, but NetworkEncryptHelper is null, please call SetNetworkEncyptHelper first");
                }

                resultBytes = encryptHelper.Decrypt(resultBytes, 0, resultBytes.Length);
                var checkResult = bccHelper.Check(resultBytes, 0, resultBytes.Length, packet.Head.bcc);
                if(checkResult == false)
                {
                    UnityEngine.Debug.LogError("Network bcc校验失败");
                    Close();
                    return;
                }
            }

            //如果有压缩标记 则执行解压缩
            if (flags.HasFlag(PacketFlag.Compress))
            {
                if(compressHelper == null)
                {
                    throw new Exception("Network Packet has Compress flag, but NetworkCompressHelper is null, please call SetNetworkCompressHelper first");
                }

                var (bytes, error) = compressHelper.Decompress(resultBytes);
                var hasError = !string.IsNullOrEmpty(error);
                resultBytes = hasError ? resultBytes : bytes;

                if (hasError)
                {
                    UnityEngine.Debug.LogError($"Network 解压缩失败:{error}");
                }
            }

            //如果有不解析标记 则直接返回
            if (flags.HasFlag(PacketFlag.NoParse))
            {
                return;
            }
            
            packet.WriteData(resultBytes);
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