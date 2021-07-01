using System;

namespace Framework.Service.Network
{
    public interface INetworkService
    {
        /// <summary>
        /// 是否已经连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 连接成功的回调
        /// </summary>
        Action<IAsyncResult> OnConnectionSuccessfulHandler { get; set; }

        /// <summary>
        /// 连接失败的回调
        /// </summary>
        Action<string> OnConnectionFailedHandler { get; set; }

        /// <summary>
        /// 关闭连接的回调
        /// </summary>
        Action OnClosedHandler { get; set; }

        /// <summary>
        /// 收到消息的回调
        /// </summary>
        Action<INetworkPacket> OnReceiveHandler { get; set; }

        /// <summary>
        /// 设置网络频道
        /// </summary>
        /// <param name="channel"></param>

        void SetNetworkChannel(INetworkChannel channel);

        /// <summary>
        /// 设置包解析帮助类
        /// </summary>
        /// <param name="packer"></param>

        void SetNetworkPackageHelper(INetworkPackageHelper packer);

        /// <summary>
        /// 设置加密帮助类
        /// </summary>
        /// <param name="encryptor"></param>
        void SetNetworkEncryptHelper(INetworkEncryptHelper encryptor);

        /// <summary>
        /// 设置消息序列化帮助类
        /// </summary>
        /// <param name="serializer"></param>
        void SetNetworkSerializeHelper(INetworkSerializeHelper serializer);

        /// <summary>
        /// 设置bcc校验帮助类
        /// </summary>
        /// <param name="bccHelper"></param>
        void SetNetworkBCCHelper(INetworkBCCHelper bccHelper);

        /// <summary>
        /// 设置消息压缩帮助类
        /// </summary>
        /// <param name="compressHelper"></param>
        void SetNetworkCompressHelper(INetworkCompressHelper compressHelper);

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="ip">服务器ip地址</param>
        /// <param name="port">服务器监听端口</param>
        void Connect(string ip, int port);

        /// <summary>
        /// 发送一个消息包
        /// </summary>
        /// <param name="packet">消息包</param>
        void Send(INetworkPacket packet);

        /// <summary>
        /// 发送一个消息
        /// </summary>
        /// <param name="id">消息id</param>
        /// <param name="bytes">消息内容</param>
        /// <param name="flag">消息标记</param>
        void Send(ushort id, byte[] bytes, PacketFlag flag = PacketFlag.Encrypt);

        /// <summary>
        /// 发送一个消息
        /// </summary>
        /// <typeparam name="T">消息数据类型</typeparam>
        /// <param name="id">消息id</param>
        /// <param name="data">消息数据类实例</param>
        /// <param name="flag">标记</param>
        void Send<T>(ushort id, T data, PacketFlag flag = PacketFlag.Encrypt) where T : class;

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();
    }
}
