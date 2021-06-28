using System.IO;

namespace Framework.Service.Network
{
    /// <summary>
    /// 消息头
    /// </summary>
    public struct PacketHead
    {
        /// <summary>
        /// Head的bytes长度
        /// </summary>
        public static readonly int HeadLength = 8;

        /// <summary>
        /// 长度
        /// </summary>
        public readonly int length;

        /// <summary>
        /// cmd
        /// </summary>
        public readonly byte cmd;

        /// <summary>
        /// act
        /// </summary>
        public readonly byte act;

        /// <summary>
        /// 标记
        /// </summary>
        public readonly byte flags;

        /// <summary>
        /// bcc校验码
        /// </summary>
        public readonly byte bcc;

        /// <summary>
        /// 消息id
        /// </summary>
        public ushort ID
        {
            get
            {
                return (ushort)((cmd << 8) | act);
            }
        }

        /// <summary>
        /// 构造消息头
        /// </summary>
        /// <param name="length">消息长度</param>
        /// <param name="cmd">cmd</param>
        /// <param name="act">act</param>
        /// <param name="flags">标记</param>
        /// <param name="bcc">bcc校验码</param>
        public PacketHead(int length, byte cmd, byte act, byte flags, byte bcc)
        {
            this.length = length;
            this.cmd = cmd;
            this.act = act;
            this.flags = flags;
            this.bcc = bcc;
        }

        /// <summary>
        /// 构造消息头
        /// </summary>
        /// <param name="length">消息体长度</param>
        /// <param name="id">消息id</param>
        /// <param name="flags">标记</param>
        /// <param name="bcc">bcc校验码</param>
        public PacketHead(int length, ushort id, byte flags, byte bcc)
        {
            this.length = length;
            cmd = (byte)(id >> 8);
            act = (byte)(id & 0x00ff);
            this.flags = flags;
            this.bcc = bcc;
        }

        /// <summary>
        /// 构造消息头
        /// </summary>
        /// <param name="bytes">bytes</param>
        public PacketHead(byte[] bytes)
        {
            if(bytes.Length < HeadLength)
            {
                throw new System.Exception("PacketHead Length must >= 8");
            }

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

                    this.length = length;
                    this.cmd = cmd;
                    this.act = act;
                    this.flags = flags;
                    this.bcc = bcc;
                }
            }
        }

        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <returns>是否为空</returns>
        public bool IsEmpty()
        {
            return length == default && cmd == default && act == default && flags == default && bcc == default;
        }

        public override string ToString()
        {
            return $"msghead length:{length} cmd:{cmd} act:{act} flags:{flags} bcc:{bcc}";
        }
    }
}
