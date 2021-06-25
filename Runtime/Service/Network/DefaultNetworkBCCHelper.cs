namespace Framework.Service.Network
{
    public class DefaultNetworkBCCHelper : INetworkBCCHelper
    {
        public byte Calculation(byte[] buffer, int offset, int length)
        {
            byte value = 0x00;
            for (int i = offset; i < offset + length; i++)
            {
                value ^= buffer[i];
            }
            return value;
        }

        public bool Check(byte[] buffer, int offset, int length, byte bcc)
        {
            var curBcc = Calculation(buffer, offset, length);
            return curBcc == bcc;
        }
    }
}