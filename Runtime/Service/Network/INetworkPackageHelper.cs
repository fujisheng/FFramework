namespace Framework.Service.Network
{
    public interface INetworkPackageHelper
    {
        byte[] Unpack(INetworkPacket packet);
        byte[] Unpack(ushort id, byte[] data, byte bcc);
        INetworkPacket Pack(byte[] bytes);
    }
}