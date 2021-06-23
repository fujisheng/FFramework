namespace Framework.Service.Network
{
    public interface ISerializer
    {
        T Serialize<T>(byte[] bytes);
        byte[] Deserialize<T>(T data);
    }
}