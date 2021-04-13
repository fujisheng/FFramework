namespace Framework.Module.Network
{
    public interface ISerializer
    {
        IMessage Serialize(byte[] bytes);
        byte[] Deserialize(IMessage packet);
    }
}