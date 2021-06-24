using System;

namespace Framework.Collections
{
    /// <summary>
    /// 环形缓冲
    /// </summary>
    /// <typeparam name="T">缓冲的数据类型</typeparam>
    public class RingBuffer<T>
    {
        int writePosition = 0;
        int readPosition = 0;
        int size = 0;

        T[] buffer;

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity { get { return buffer.Length; } }

        /// <summary>
        /// 大小
        /// </summary>
        public int Size { get { return size; } }

        public RingBuffer() : this(4) { }

        public RingBuffer(int capacity)
        {
            buffer = new T[capacity];
        }

        /// <summary>
        /// 获取buffer
        /// </summary>
        /// <returns></returns>
        public T[] GetBuffer()
        {
            return buffer;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data"></param>
        public void Write(T data)
        {
            size++;
            buffer[writePosition] = data;
            writePosition = (writePosition + 1) % Capacity;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            if(size == 0)
            {
                return default(T);
            }

            T data = buffer[readPosition];
            readPosition = (readPosition + 1) % Capacity;
            size--;
            return data;
        }

        /// <summary>
        /// 写入一串数据
        /// </summary>
        /// <param name="datas"></param>
        public void Write(T[] datas)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                Write(datas[i]);
            }
        }

        /// <summary>
        /// 读取一串数据
        /// </summary>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        public void Read(int length, T[] buffer)
        {
            var l = Math.Min(length, buffer.Length);
            for(int i = 0; i < l; i++)
            {
                buffer[i] = Read();
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            readPosition = 0;
            writePosition = 0;
            size = 0;
        }

        public override string ToString()
        {
            return $"readPosition:{readPosition} writePosition:{writePosition}";
        }
    }
}
