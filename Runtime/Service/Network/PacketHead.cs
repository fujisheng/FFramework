using System;

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
                throw new Exception("PacketHead Length must >= 8");
            }

            //消息长度
            int length = Utility.Converter.GetInt32(bytes);
            //cmd
            byte cmd = bytes[4];
            //act
            byte act = bytes[5];
            //flags
            byte flags = bytes[6];
            //bcc
            byte bcc = bytes[7];

            this.length = length;
            this.cmd = cmd;
            this.act = act;
            this.flags = flags;
            this.bcc = bcc;
        }

        /// <summary>
        /// 构造消息头
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <param name="lenght">消息头长度</param>
        public unsafe PacketHead(byte* bytes, int lenght)
        {
            if (lenght < HeadLength)
            {
                throw new Exception("PacketHead Length must >= 8");
            }

            //消息长度
            int length = Utility.Converter.GetInt32(bytes[0], bytes[1], bytes[2], bytes[3]);
            //cmd
            byte cmd = bytes[4];
            //act
            byte act = bytes[5];
            //flags
            byte flags = bytes[6];
            //bcc
            byte bcc = bytes[7];

            this.length = length;
            this.cmd = cmd;
            this.act = act;
            this.flags = flags;
            this.bcc = bcc;
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
