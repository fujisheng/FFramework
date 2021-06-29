using Framework.Collections;

namespace Framework.Service.Network
{
    internal class BytesBuffer : RingBuffer<byte>
    {
        public BytesBuffer(int capacity) : base(capacity) { }

        /// <summary>
        /// 读取一串数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        internal unsafe void Read(byte* buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                buffer[i] = Read();
            }
        }
    }
}
