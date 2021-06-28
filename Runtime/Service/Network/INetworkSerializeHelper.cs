using System;

namespace Framework.Service.Network
{
    public interface INetworkSerializeHelper
    {
        T Deserialize<T>(byte[] bytes);
        object DeserializeNonGeneric(Type type, byte[] bytes);
        byte[] Serialize<T>(T data);
        byte[] SerializeNonGeneric(object data);
    }
}