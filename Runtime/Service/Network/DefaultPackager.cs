using System;
using System.IO;
using System.Linq;

namespace Framework.Service.Network
{
    public class DefaultPackager : IPackager
    {
        IObjectPool<IPacket> packetPool;
        private DefaultPackager() { }

        /// <summary>
        /// 通过PacketPool构造Packager
        /// </summary>
        /// <param name="packetPool"></param>
        public DefaultPackager(IObjectPool<IPacket> packetPool)
        {
            this.packetPool = packetPool;
        }

        public static PacketHead ReadHead(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                using (var reader = new BinaryReader(stream))
                {
                    stream.Seek(0, SeekOrigin.End);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    //消息长度
                    int length = reader.ReadInt32();
                    //cmd
                    byte cmd = reader.ReadByte();
                    //act
                    byte act = reader.ReadByte();
                    //flags
                    byte flags = reader.ReadByte();
                    //bcc
                    byte bcc = reader.ReadByte();
                    ushort msgId = (ushort)((cmd << 8) | act);

                    return new PacketHead(msgId, cmd, act, flags, bcc);
                }
            }
        }

        /// <summary>
        /// 将消息打包成packet
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public IPacket Pack(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                using(var reader = new BinaryReader(stream))
                {
                    stream.Seek(0, SeekOrigin.End);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);

                    //消息长度
                    int length = reader.ReadInt32();
                    //cmd
                    byte cmd = reader.ReadByte();
                    //act
                    byte act = reader.ReadByte();
                    //flags
                    byte flags = reader.ReadByte();
                    //bcc
                    byte bcc = reader.ReadByte();

                    UnityEngine.Debug.Log($"length:{length} cmd:{cmd} act:{act} flags:{flags} bcc:{bcc}");
                    //UnityEngine.Debug.Log(BitConverter.ToInt32(reversL, reversL.Length));
                    //data
                    byte[] data = reader.ReadBytes(length);

                    ushort msgId = (ushort)((cmd << 8) | act);
                    IPacket packet = packetPool.Pop();

                    packet.Head = new PacketHead(msgId, cmd, act, flags, bcc);
                    packet.Body = data;
                    reader.Close();
                    return packet;
                }
            }
        }

        /// <summary>
        /// 将packet解包成bytes
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public byte[] Unpack(IPacket packet)
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
                    writer.Write(packet.Body);
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
        public byte[] Unpack(ushort id, byte[] data, byte bcc)
        {
            var packet = packetPool.Pop();
            var flags = (byte)1;
            packet.Head = new PacketHead(data.Length, id, flags, bcc);
            packet.Body = data;
            return Unpack(packet);
        }
    }
}