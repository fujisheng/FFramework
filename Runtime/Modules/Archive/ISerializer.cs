using System;

namespace Framework.Module.Archive
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T obj);
        byte[] Serialize(object obj);
        T Deserialize<T>(byte[] data);
        object Deserialize(byte[] data, Type type);
    }
}