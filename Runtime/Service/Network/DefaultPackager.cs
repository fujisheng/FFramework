using System;
using System.IO;

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

        /// <summary>
        /// 将消息打包成packet
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public IPacket Pack(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    BinaryReader reader = new BinaryReader(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.End);
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    int messageLength = reader.ReadInt32();
                    byte cmd = reader.ReadByte();
                    byte act = reader.ReadByte();
                    ushort errorCode = reader.ReadUInt16();
                    byte[] data = reader.ReadBytes(messageLength);
                    int msgId = (cmd << 8) + act;
                    IPacket packet = packetPool.Pop();
                    packet.Id = msgId;
                    packet.Length = messageLength;
                    packet.Bytes = data;
                    reader.Close();
                    return packet;
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogErrorFormat("UnpackMessage Error:{0}", ex.ToString());
                }
                return null;
            }
        }

        /// <summary>
        /// 将packet解包成bytes
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public byte[] Unpack(IPacket packet)
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Position = 0;
                BinaryWriter writer = new BinaryWriter(memoryStream);
                int msglen = packet.Length;
                //消息长度
                writer.Write(msglen);
                byte cmd = (byte)(packet.Id >> 8);
                byte act = (byte)(packet.Id & 0x00ff);
                //消息ID
                writer.Write(cmd);
                writer.Write(act);
                //错误码
                writer.Write((ushort)0);
                //消息内容
                writer.Write(packet.Bytes);
                writer.Flush();
                byte[] payload = memoryStream.ToArray();

                packet.Release();
                packetPool.Push(packet);
                return payload;
            }
        }
    }
}