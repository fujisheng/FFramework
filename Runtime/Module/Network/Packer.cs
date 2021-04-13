using System;
using System.IO;
using UnityEngine;

namespace Framework.Module.Network
{
    public class Packer : IPacker
    {
        MemoryStream memoryStream;
        IObjectPool<IMessage> messagePool;

        public void SetMessagePool(IObjectPool<IMessage> messagePool)
        {
            this.messagePool = messagePool;
        }

        /// <summary>
        /// 将bytes转换为message
        /// </summary>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public IMessage Unpack(byte[] bytes)
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
                    IMessage message = messagePool.Pop();
                    message.Id = msgId;
                    message.Length = messageLength;
                    message.Bytes = data;
                    reader.Close();
                    return message;
                }
                catch(Exception ex)
                {
                    Debug.LogErrorFormat("UnpackMessage Error:{0}", ex.ToString());
                }
                return null;
            }
        }

        /// <summary>
        /// 将mssage转换为bytes
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] Pack(IMessage message)
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
                messagePool.Push(message);
                return payload;
            }
        }

        public void Close()
        {
            memoryStream.Close();
        }
    }
}