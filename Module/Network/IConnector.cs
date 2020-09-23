using System;

namespace Framework.Module.Network
{
    public interface IConnector
    {
        bool IsConnected { get; }
        Action OnConnented { get; set; }
        Action OnConnectionFailed { get; set; }
        Action OnClosed { get; set; }
        Action<byte[]> OnReceive { get; set; }
        void OnUpdate();
        void Connect(string ip, int port);
        void Send(byte[] bytes);
        void Close();
    }
}