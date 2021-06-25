using ProtoBuf;
using System;
using System.IO;

namespace Framework.Service.Network
{
    public class ProtobufSerializer : INetworkSerializeHelper
    {
        public byte[] Serialize<T>(T data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, data);
                byte[] bytes = ms.ToArray();
                return bytes;
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (MemoryStream ms1 = new MemoryStream(bytes))
            {
                var p1 = Serializer.Deserialize<T>(ms1);
                return p1;
            }
        }
    }
}
