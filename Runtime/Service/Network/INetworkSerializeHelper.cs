namespace Framework.Service.Network
{
    public interface INetworkSerializeHelper
    {
        T Deserialize<T>(byte[] bytes);
        byte[] Serialize<T>(T data);
    }
}