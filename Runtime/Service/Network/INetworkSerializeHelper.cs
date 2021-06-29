namespace Framework.Service.Network
{
    public interface INetworkSerializeHelper
    {
        T Deserialize<T>(byte[] bytes) where T : class;
        byte[] Serialize<T>(T data) where T : class;
    }
}