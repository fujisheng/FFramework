namespace Framework.Service.Network
{
    public interface INetworkEncryptHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="buffer">数据缓冲</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">数据长度</param>
        /// <returns>加密后的数据</returns>
        byte[] Encrypt(byte[] buffer, int offset, int length);

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer">数据缓冲</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">数据长度</param>
        /// <returns>解密后的数据</returns>
        byte[] Decrypt(byte[] buffer, int offset, int length);
    }
}
