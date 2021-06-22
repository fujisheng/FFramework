using ProtoBuf;
using System.IO;

namespace Framework.Service.Network
{
    public class ProtobufSerializer : ISerializer
    {
        public byte[] Deserialize<T>(T packet)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, packet);
                byte[] data = ms.ToArray();
                return data;
            }
        }

        public T Serialize<T>(byte[] bytes)
        {
            using (MemoryStream ms1 = new MemoryStream(bytes))
            {
                var p1 = Serializer.Deserialize<T>(ms1);
                return p1;
            }
        }
    }
}
