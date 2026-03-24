// UnityLZ4Compression.cs
namespace Framework.Module.Archive
{
    public class EmptyCompressionProvider : ICompressionProvider
    {
        public byte[] Compress(byte[] rawData)
        {
            return rawData;
        }
        public byte[] Decompress(byte[] compressedData)
        {
            return compressedData;
        }
    }
}