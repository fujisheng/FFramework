using System;
using System.IO;

namespace Framework.Service.Network
{
    public class Packer : IPacker
    {
        MemoryStream memoryStream;
        IObjectPool<IPacket> packetPool;

        public void SetMessagePool(IObjectPool<IPacket> packetPool)
        {
            this.packetPool = packetPool;
        }

        /// <summary>
        /// 将bytes转换为message
        /// </summary>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public IPacket Unpack(byte[] bytes)
        {
            using (memoryStream = new MemoryStream())
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
                    IPacket message = packetPool.Pop();
                    message.Id = msgId;
                    message.Length = messageLength;
                    message.Bytes = data;
                    reader.Close();
                    return message;
                }
                catch(Exception ex)
                {
                    UnityEngine.Debug.LogErrorFormat("UnpackMessage Error:{0}", ex.ToString());
                }
                return null;
            }
        }

        /// <summary>
        /// 将mssage转换为bytes
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] Pack(IPacket message)
        {
            using (memoryStream = new MemoryStream())
            {
                memoryStream.Position = 0;
                BinaryWriter writer = new BinaryWriter(memoryStream);
                int msglen = message.Length;
                //消息长度
                writer.Write(msglen);
                byte cmd = (byte)(message.Id >> 8);
                byte act = (byte)(message.Id & 0x00ff);
                //消息ID
                writer.Write(cmd);
                writer.Write(act);
                //错误码
                writer.Write((ushort)0);
                //消息内容
                writer.Write(message.Bytes);
                writer.Flush();
                byte[] payload = memoryStream.ToArray();

                message.Clear();
                packetPool.Push(message);
                return payload;
            }
        }

        public void Close()
        {
            memoryStream.Close();
        }
    }
}