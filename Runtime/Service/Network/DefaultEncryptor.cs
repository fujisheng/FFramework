namespace Framework.Service.Network
{
    public class DefaultEncryptor : IEncryptor
    {
        byte[] encryptSeed;
        byte[] decryptSeed;

        /// <summary>
        /// 通过加解密种子构造
        /// </summary>
        /// <param name="encryptSeed">加密种子</param>
        /// <param name="decryptSeed">解密种子</param>
        public DefaultEncryptor(byte[] encryptSeed, byte[] decryptSeed)
        {
            this.encryptSeed = encryptSeed;
            this.decryptSeed = decryptSeed;
        }

        /// <summary>
        /// 设置加解密种子
        /// </summary>
        /// <param name="encryptSeed">加密种子</param>
        /// <param name="decryptSeed">解密种子</param>
        public void SetSeed(byte[] encryptSeed, byte[] decryptSeed)
        {
            this.encryptSeed = encryptSeed;
            this.decryptSeed = decryptSeed;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="buffer">数据缓冲</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">数据长度</param>
        /// <returns>加密后的数据</returns>
        public byte[] Encrypt(byte[] buffer, int offset, int length)
        {
            if(length <= offset)
            {
                return buffer;
            }

            var k = 0;
            var c = 0;
            for (int i = offset; i < length; i++)
            {
                k %= 4;
                var x = (buffer[i] ^ encryptSeed[k]) + c;
                k++;
                c = x;
                buffer[i] = (byte)c;
            }
            return buffer;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="buffer">数据缓冲</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">数据长度</param>
        /// <returns>解密后的数据</returns>
        public byte[] Decrypt(byte[] buffer, int offset, int length)
        {
            if(length <= offset)
            {
                return buffer;
            }

            var k = 0;
            var c = 0;
            for (int i = offset; i < length; i++)
            {
                k %= 4;
                var x = (buffer[i] - c) ^ decryptSeed[k];
                k++;
                c = buffer[i];
                buffer[i] = (byte)x;
            }
            return buffer;
        }
    }
}