using System;

namespace Framework.Service.Network
{
    public interface INetworkChannel
    {
        bool IsConnected { get; }
        Action<IAsyncResult> OnConnented { get; set; }
        Action OnConnectionFailed { get; set; }
        Action OnClosed { get; set; }
        Action<byte[]> OnReceive { get; set; }
        void OnUpdate();
        void Connect(string ip, int port);
        void Send(byte[] bytes);
        void Close();
    }
}