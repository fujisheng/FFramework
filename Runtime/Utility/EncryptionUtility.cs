using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Framework
{
    public static partial class Utility
    {
        public static class Encryption
        {
            public const string DEFAULT_128_64STRING_KEY = "AAECAwQFBgcICQoLDA0ODw==";

            /// <summary>
            /// AES加密(无向量)
            /// </summary>
            /// <param name="data">被加密的bytes</param>
            /// <param name="key">密钥</param>
            /// <returns>加密后的bytes</returns>
            public static byte[] AESEncrypt(byte[] data, string key = DEFAULT_128_64STRING_KEY)
            {
                MemoryStream mStream = new MemoryStream();
                RijndaelManaged aes = new RijndaelManaged();

                byte[] bKey = new Byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);

                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.Key = bKey;
                CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
                try
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    return mStream.ToArray();
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                    return null;
                }
                finally
                {
                    cryptoStream.Close();
                    mStream.Close();
                    aes.Clear();
                }
            }

            /// <summary>
            /// AES加密(无向量)
            /// </summary>
            /// <param name="data">被加密明文</param>
            /// <param name="key">密钥</param>
            /// <returns>密文</returns>
            public static string AESEncrypt(string data, string key = DEFAULT_128_64STRING_KEY)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);

                return Convert.ToBase64String(AESEncrypt(bytes, key));
            }

            /// <summary>
            /// AES解密(无向量)
            /// </summary>
            /// <param name="data">被加密的bytes</param>
            /// <param name="key">密钥</param>
            /// <returns>明文Bytes</returns>
            public static byte[] AESDecrypt(byte[] data, string Key = DEFAULT_128_64STRING_KEY)
            {

                byte[] bKey = new byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);

                MemoryStream mStream = new MemoryStream(data);
                RijndaelManaged aes = new RijndaelManaged();
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 128;
                aes.Key = bKey;
                CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
                try
                {
                    byte[] tmp = new byte[data.Length + 32];
                    int len = cryptoStream.Read(tmp, 0, data.Length + 32);
                    byte[] ret = new byte[len];
                    Array.Copy(tmp, 0, ret, 0, len);
                    return ret;
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                    return null;
                }
                finally
                {
                    cryptoStream.Close();
                    mStream.Close();
                    aes.Clear();
                }
            }

            /// <summary>
            /// AES解密(无向量)
            /// </summary>
            /// <param name="Data">被加密的明文</param>
            /// <param name="key">密钥</param>
            /// <returns>明文</returns>
            public static string AESDecrypt(string Data, string Key = DEFAULT_128_64STRING_KEY)
            {
                byte[] encryptedBytes = Convert.FromBase64String(Data);
                byte[] bytes = AESDecrypt(encryptedBytes, Key);
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}