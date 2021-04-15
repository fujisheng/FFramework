namespace Framework.Service.Network
{
    public interface ISerializer
    {
        IMessage Serialize(byte[] bytes);
        byte[] Deserialize(IMessage packet);
    }
}