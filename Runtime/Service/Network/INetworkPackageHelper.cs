namespace Framework.Service.Network
{
    public interface INetworkPackageHelper
    {
        byte[] Unpack(INetworkPacket packet);
        byte[] Unpack(ushort id, byte flags, byte bcc, byte[] data);
        INetworkPacket Pack(byte[] bytes);
    }
}