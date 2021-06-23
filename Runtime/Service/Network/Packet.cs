using System;

namespace Framework.Service.Network
{
    public struct Packet : IPacket
    {
        public int Id { get; set; }
        public byte[] Bytes { get; set; }
        public int Length { get; set; }

        public void Release()
        {
            Array.Clear(Bytes, 0, Bytes.Length);
            Length = 0;
            Id = 0;
        }
    }
}