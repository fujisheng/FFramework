namespace Framework.Collections
{
    /// <summary>
    /// �����Զ����ŵĻ��λ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GrowingRingBuffer<T> : RingBuffer<T>
    {
        int originalCapacity;

        public GrowingRingBuffer() : this(4) { }

        public GrowingRingBuffer(int startCapacity) : base(startCapacity)
        {
            originalCapacity = startCapacity;
        }

        /// <summary>
        /// ���һ��
        /// </summary>
        /// <param name="item"></param>
        public new void Put(T item)
        {
            if (tail == head && size != 0)
            {
                T[] newArray = new T[buffer.Length + originalCapacity];
                for (int i = 0; i < Capacity; i++)
                {
                    newArray[i] = buffer[i];
                }
                buffer = newArray;
                tail = (head + size) % Capacity;
                AddToBuffer(item, false);
            }
            else
            {
                AddToBuffer(item, false);
            }
        }
    }
}
