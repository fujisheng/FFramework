namespace Framework.Service.Network
{
    public interface IPackager
    {
        byte[] Unpack(IPacket packet);
        IPacket Pack(byte[] bytes);
    }
}