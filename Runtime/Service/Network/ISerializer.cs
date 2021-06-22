using System.IO;

namespace Framework.Service.Network
{
    public interface ISerializer
    {
        T Serialize<T>(byte[] destination);
        byte[] Deserialize<T>(T packet);
    }
}