namespace Framework.Module.Archive
{
    public class EmptyEncryptor : IEncryptionProvider
    {
        public byte[] Encrypt(byte[] rawData)
        {
            return rawData;
        }
        public byte[] Decrypt(byte[] encryptedData)
        {
            return encryptedData;
        }
    }
}