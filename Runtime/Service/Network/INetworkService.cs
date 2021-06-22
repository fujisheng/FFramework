namespace Framework.Service.Network
{
    public interface INetworkService
    {
        void SetPacker(IPacker packer);
        void SetNetworkChannel(INetworkChannel channel);

        void Connect(string ip, int port);

        void Send(IPacket packet);
    }
}
