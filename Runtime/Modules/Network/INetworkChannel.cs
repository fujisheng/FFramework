using System;

namespace Framework.Module.Network
{
    public interface INetworkChannel
    {
        bool IsConnected { get; }
        Action<IAsyncResult> OnConnectionSuccessfulHandler { get; set; }
        Action<string> OnConnectionFailedHandler { get; set; }
        Action<INetworkPacket> OnReceiveHandler { get; set; }
        Action OnCloseHandler { get; set; }

        void OnUpdate();
        void Connect(string ip, int port);
        void Send(byte[] bytes);
        void Close();
    }
}