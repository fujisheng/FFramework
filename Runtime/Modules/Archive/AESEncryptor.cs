using System.Security.Cryptography;

namespace Framework.Module.Archive
{
    public class AESEncryptor : IEncryptionProvider
    {
        readonly byte[] key;
        readonly byte[] iv;

        public AESEncryptor(byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv = iv;
        }

        public byte[] Encrypt(byte[] rawData)
        {
            using var aes = Aes.Create();
            using var encryptor = aes.CreateEncryptor(key, iv);
            return encryptor.TransformFinalBlock(rawData, 0, rawData.Length);
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            using var aes = Aes.Create();
            using var decryptor = aes.CreateDecryptor(key, iv);
            return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}