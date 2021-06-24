using System;

namespace Framework.Collections
{
    /// <summary>
    /// ���λ���
    /// </summary>
    /// <typeparam name="T">�������������</typeparam>
    public class RingBuffer<T>
    {
        int writePosition = 0;
        int readPosition = 0;
        int size = 0;

        T[] buffer;

        /// <summary>
        /// ����
        /// </summary>
        public int Capacity { get { return buffer.Length; } }

        /// <summary>
        /// ��С
        /// </summary>
        public int Size { get { return size; } }

        public RingBuffer() : this(4) { }

        public RingBuffer(int capacity)
        {
            buffer = new T[capacity];
        }

        /// <summary>
        /// ��ȡbuffer
        /// </summary>
        /// <returns></returns>
        public T[] GetBuffer()
        {
            return buffer;
        }

        /// <summary>
        /// д������
        /// </summary>
        /// <param name="data"></param>
        public void Write(T data)
        {
            size++;
            buffer[writePosition] = data;
            writePosition = (writePosition + 1) % Capacity;
        }

        /// <summary>
        /// ��ȡ����
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
        /// д��һ������
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
        /// ��ȡһ������
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
        /// ���
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
