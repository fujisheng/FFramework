namespace Framework.Module.Archive
{
    public interface IEncryptionProvider
    {
        byte[] Encrypt(byte[] rawData);
        byte[] Decrypt(byte[] encryptedData);
    }
}