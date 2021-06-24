using System;

namespace Framework.Service.Network
{
    public interface INetworkService
    {
        Action<IAsyncResult> OnConnectionSuccessfulHandler { get; set; }
        Action<string> OnConnectionFailedHandler { get; set; }
        Action OnClosedHandler { get; set; }
        Action<IPacket> OnReceiveHandler { get; set; }
        void SetPackager(IPackager packer);
        void SetNetworkChannel(INetworkChannel channel);
        void SetEncryptor(IEncryptor encryptor);
        void SetSerializer(ISerializer serializer);
        void Connect(string ip, int port);
        void Send(IPacket packet);
        void Send(byte[] bytes);
        void Send<T>(ushort id, T data);
        void Close();
    }
}
