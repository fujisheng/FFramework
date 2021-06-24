using System;

namespace Framework.Service.Network
{
    public struct Packet : IPacket
    {
        public PacketHead Head { get; set; }
        public byte[] Data { get; set; }
       
        public void Release()
        {
            Array.Clear(Data, 0, Data.Length);
            Head = default;
        }
    }
}