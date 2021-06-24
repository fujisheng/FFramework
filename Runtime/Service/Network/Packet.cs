using System;

namespace Framework.Service.Network
{
    public struct Packet : IPacket
    {
        public PacketHead Head { get; set; }
        public byte[] Body { get; set; }
       
        public void Release()
        {
            Array.Clear(Body, 0, Body.Length);
            Head = default;
        }
    }
}