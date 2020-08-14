using System;
using System.Collections.Generic;

namespace KoitanLib.Collections
{
    /// <summary>
    /// 優先度付きキュー
    /// </summary>
    /// <typeparam name="TValue">要素の型</typeparam>
    /// <typeparam name="TPriority">優先度の型</typeparam>
    public class PriorityQueue<TValue, TPriority> where TPriority : IComparable<TPriority>
    {
        public class ValueAndPriority
        {
            public TValue value;
            public TPriority priority;

            public ValueAndPriority(TValue value, TPriority priority)
            {
                this.value = value;
                this.priority = priority;
            }
        }

        List<ValueAndPriority> buffer;

        //初期化
        public PriorityQueue()
        {
            this.buffer = new List<ValueAndPriority>();
        }

        public PriorityQueue(int capacity)
        {
            this.buffer = new List<ValueAndPriority>(capacity);
        }

        /// <summary>
        /// ヒープ化されている配列リストに新しい要素を追加する。
        /// </summary>
        /// <param name="array">対象の配列リスト</param>
        static void PushHeap(List<ValueAndPriority> array, ValueAndPriority elem)
        {
            int n = array.Count;
            array.Add(elem);

            while (n != 0)
            {
                int i = (n - 1) / 2;
                if (array[n].priority.CompareTo(array[i].priority) > 0)
                {
                    ValueAndPriority tmp = array[n]; array[n] = array[i]; array[i] = tmp;
                }
                n = i;
            }
        }

        /// <summary>
        /// ヒープから最大値を削除する。
        /// </summary>
        /// <param name="array">対象の配列リスト</param>
        static void PopHeap(List<ValueAndPriority> array)
        {
            int n = array.Count - 1;
            array[0] = array[n];
            array.RemoveAt(n);

            for (int i = 0, j; (j = 2 * i + 1) < n;)
            {
                if ((j != n - 1) && (array[j].priority.CompareTo(array[j + 1].priority) < 0))
                    j++;
                if (array[i].priority.CompareTo(array[j].priority) < 0)
                {
                    ValueAndPriority tmp = array[j]; array[j] = array[i]; array[i] = tmp;
                }
                i = j;
            }
        }

        /// <summary>
        /// 要素のプッシュ。
        /// </summary>
        /// <param name="value">要素</param>
        /// <param name="priority">優先度</param>
        public void Push(TValue value, TPriority priority)
        {
            PushHeap(this.buffer, new ValueAndPriority(value, priority));
        }

        /// <summary>
        /// 要素を1つポップ。
        /// </summary>
        public TValue Pop()
        {
            TValue tmp = Top;
            PopHeap(this.buffer);
            return tmp;
        }

        /// <summary>
        /// 先頭要素の読み出し。
        /// </summary>
        public TValue Top
        {
            get { return this.buffer[0].value; }
        }

        public int Count
        {
            get { return this.buffer.Count; }
        }
    }
}
