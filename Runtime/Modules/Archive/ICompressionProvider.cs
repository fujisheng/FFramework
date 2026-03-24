namespace Framework.Module.Archive
{
    public interface ICompressionProvider
    {
        byte[] Compress(byte[] rawData);
        byte[] Decompress(byte[] compressedData);
    }
}