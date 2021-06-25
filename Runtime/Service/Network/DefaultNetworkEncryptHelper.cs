using System;

namespace Framework.Service.Network
{
    public class DefaultNetworkEncryptHelper : INetworkEncryptHelper
    {
        const uint encryptA = 214003;
        const uint encryptB = 2531012;

        uint encryptSeed;
        uint decryptSeed;

        /// <summary>
        /// 通过加解密种子构造
        /// </summary>
        /// <param name="encryptSeed">加密种子</param>
        /// <param name="decryptSeed">解密种子</param>
        public DefaultNetworkEncryptHelper(uint encryptSeed, uint decryptSeed)
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
            //先计算新的加密种子
            encryptSeed = encryptSeed * encryptA + encryptB;

            if (length <= offset)
            {
                return buffer;
            }

            //计算加密后的数据
            var seedBytes = BitConverter.GetBytes(encryptSeed);
            var k = 0;
            var c = 0;
            for (int i = offset; i < length; i++)
            {
                k %= 4;
                var x = (buffer[i] ^ seedBytes[k]) + c;
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
            //先计算新的解密种子
            decryptSeed = decryptSeed * encryptA + encryptB;

            if(length <= offset)
            {
                return buffer;
            }

            //计算解密后的数据
            var seedBytes = BitConverter.GetBytes(decryptSeed);
            var k = 0;
            var c = 0;
            for (int i = offset; i < length; i++)
            {
                k %= 4;
                var x = (buffer[i] - c) ^ seedBytes[k];
                k++;
                c = buffer[i];
                buffer[i] = (byte)x;
            }
            return buffer;
        }
    }
}