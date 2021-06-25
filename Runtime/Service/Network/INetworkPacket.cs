namespace Framework.Service.Network
{
    public interface INetworkPacket
    {
        ushort ID { get; }
        int Length { get; }
        PacketHead Head { get; }
        byte[] Data { get; }

        void WriteHead(PacketHead head);
        void WriteData(byte[] data);
        void Release();
    }
}