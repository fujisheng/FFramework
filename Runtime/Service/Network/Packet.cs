using System;

namespace Framework.Service.Network
{
    public struct Packet : INetworkPacket
    {
        public PacketHead Head { get; private set; }
        public byte[] Data { get; private set; }

        public ushort ID => Head.ID;

        public int Length => Head.length;

        public Packet(PacketHead head)
        {
            this.Head = head;
            Data = null;
        }

        public void WriteHead(PacketHead head)
        {
            this.Head = head;
        }

        public void WriteData(byte[] bytes)
        {
            this.Data = bytes;
        }
       
        public void Release()
        {
            Array.Clear(Data, 0, Data.Length);
            Head = default;
        }
    }
}