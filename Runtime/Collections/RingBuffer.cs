using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Collections
{
    /// <summary>
    /// 环形缓冲
    /// </summary>
    /// <typeparam name="T">缓冲的数据类型</typeparam>
    public class RingBuffer<T> : IEnumerable<T>, IEnumerable, ICollection<T>, ICollection 
    {
        protected int head = 0;
        protected int tail = 0;
        protected int size = 0;

        protected T[] buffer;
        readonly bool allowOverflow;

        /// <summary>
        /// 是否允许溢出
        /// </summary>
        public bool AllowOverflow { get { return allowOverflow; } }

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity { get { return buffer.Length; } }

        /// <summary>
        /// 大小
        /// </summary>
        public int Size { get { return size; } }

        public RingBuffer() : this(4) { }

        public RingBuffer(int capacity) : this(capacity, false) { }

        public RingBuffer(int capacity, bool overflow)
        {
            buffer = new T[capacity];
            allowOverflow = overflow;
        }

        /// <summary>
        /// 获取下一项
        /// </summary>
        /// <returns></returns>
        public T Get() 
        {
            if (size == 0)
            { 
                throw new System.InvalidOperationException("Buffer is empty"); 
            }

            T item = buffer[head];
            head = (head + 1) % Capacity;
            size--;
            return item;
        }

        /// <summary>
        /// 添加一项数据
        /// </summary>
        /// <param name="item"></param>
        public void Put(T item) 
        {
            if(tail == head && size != 0) 
            {
                if(allowOverflow) 
                {
                    AddToBuffer(item, true);
                }
                else 
                {
                    throw new InvalidOperationException("The RingBuffer is full");
                }
            }
            else 
            {
                AddToBuffer(item, false);
            }
        }

        protected void AddToBuffer(T toAdd, bool overflow) 
        {
            if(overflow) 
            {
                head = (head + 1) % Capacity;
            }
            else 
            {
                size++;
            }
            buffer[tail] = toAdd;
            tail = (tail + 1) % Capacity;
        }

        #region IEnumerable Members
        public IEnumerator<T> GetEnumerator() 
        {
            int _index = head;
            for(int i = 0; i < size; i++, _index = (_index + 1) % Capacity)
            {
                yield return buffer[_index];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() 
        {
            return (IEnumerator)GetEnumerator();
        }
        #endregion

        #region ICollection<T> Members
        public int Count { get { return size; } }
        public bool IsReadOnly { get { return false; } }

        public void Add(T item) 
        {
            Put(item);
        }
        
        /// <summary>
        /// 判断是否包含某一项
        /// </summary>
        /// <param name="item"></param>
        /// <returns>是否包含</returns>
        public bool Contains(T item) 
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            int index = head;
            for(int i = 0; i < size; i++, index = (index + 1) % Capacity) 
            {
                if (comparer.Equals(item, buffer[index]))
                { 
                    return true; 
                }
            }
            return false;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear() 
        {
            for(int i = 0; i < Capacity; i++) 
            {
                buffer[i] = default(T);
            }
            head = 0;
            tail = 0;
            size = 0;
        }

        /// <summary>
        /// 拷贝到一个数组
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex) 
        {
            int index = head;
            for(int i = 0; i < size; i++, arrayIndex++, index = (index + 1) % Capacity) 
            {
                array[arrayIndex] = buffer[index];
            }
        }

        /// <summary>
        /// 移除某一项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item) 
        {
            int index = head;
            int removeIndex = 0;
            bool foundItem = false;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for(int i = 0; i < size; i++, index = (index + 1) % Capacity) 
            {
                if(comparer.Equals(item, buffer[index])) 
                {
                    removeIndex = index;
                    foundItem = true;
                    break;
                }
            }

            if(foundItem) 
            {
                T[] newBuffer = new T[size - 1];
                index = head;
                bool pastItem = false;

                for(int i = 0; i < size - 1; i++, index = (index + 1) % Capacity) 
                {
                    if(index == removeIndex) 
                    {
                        pastItem = true;
                    }
                    if(pastItem)
                    {
                        newBuffer[index] = buffer[(index + 1) % Capacity];
                    }
                    else 
                    {
                        newBuffer[index] = buffer[index];
                    }
                }

                size--;
                buffer = newBuffer;
                return true;
            }
            return false;
        }
        #endregion

        #region ICollection Members
        public Object SyncRoot { get { return this; } }

        public bool IsSynchronized { get { return false; } }
        void ICollection.CopyTo(Array array, int index) 
        {
            CopyTo((T[])array, index);
        }
        #endregion
    }
}
