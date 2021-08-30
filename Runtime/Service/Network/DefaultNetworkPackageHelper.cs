using System.IO;

namespace Framework.Service.Network
{
    public class DefaultNetworkPackageHelper : INetworkPackageHelper
    {
        IObjectPool<INetworkPacket> packetPool;
        private DefaultNetworkPackageHelper() { }

        /// <summary>
        /// 通过PacketPool构造Packager
        /// </summary>
        /// <param name="packetPool"></param>
        public DefaultNetworkPackageHelper(IObjectPool<INetworkPacket> packetPool)
        {
            this.packetPool = packetPool;
        }

        /// <summary>
        /// 将消息打包成packet
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public INetworkPacket Pack(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            using (var reader = new BinaryReader(stream))
            {
                stream.Seek(0, SeekOrigin.End);
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var headBytes = reader.ReadBytes(PacketHead.HeadLength);
                var head = new PacketHead(headBytes);
                byte[] data = reader.ReadBytes(head.length);
                INetworkPacket packet = packetPool.Pop();
                packet.WriteHead(head);
                packet.WriteData(data);
                reader.Close();
                return packet;
            }
        }

        /// <summary>
        /// 将packet解包成bytes
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public byte[] Unpack(INetworkPacket packet)
        {
            using (var stream = new MemoryStream())
            {
                stream.Position = 0;
                using (var writer = new BinaryWriter(stream))
                {
                    int msglen = packet.Head.length;

                    //消息长度
                    writer.Write(packet.Head.length);

                    //消息ID
                    writer.Write(packet.Head.cmd);
                    writer.Write(packet.Head.act);

                    //flag
                    writer.Write(packet.Head.flags);

                    //BCC校验码
                    writer.Write(packet.Head.bcc);

                    //消息内容
                    writer.Write(packet.Data);
                    writer.Flush();
                    byte[] result = stream.ToArray();

                    packet.Release();
                    packetPool.Push(packet);
                    return result;
                }
            }
        }

        /// <summary>
        /// 将id和data解包成bytes
        /// </summary>
        /// <param name="id">包的id</param>
        /// <param name="data">包的数据</param>
        /// <returns>解包后的数据</returns>
        public byte[] Unpack(ushort id, byte flags, byte bcc, byte[] data)
        {
            var packet = packetPool.Pop();
            packet.WriteHead(new PacketHead(data.Length, id, flags, bcc));
            packet.WriteData(data);
            return Unpack(packet);
        }
    }
}