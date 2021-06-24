namespace Framework.Service.Network
{
    public interface IPackager
    {
        byte[] Unpack(IPacket packet);
        byte[] Unpack(ushort id, byte[] data, byte bcc);
        IPacket Pack(byte[] bytes);
    }
}