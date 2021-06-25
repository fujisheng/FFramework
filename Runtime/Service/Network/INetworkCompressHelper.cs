namespace Framework.Service.Network
{
    public interface INetworkCompressHelper
    {
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns></returns>
        (byte[] bytes, string error) Decompress(byte[] bytes);

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns></returns>
        (byte[] bytes, string error) Compress(byte[] bytes);
    }
}