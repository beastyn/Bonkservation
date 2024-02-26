namespace BonkUtils
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;

    public class FixedSizeQueue<T>
    {
        readonly ConcurrentQueue<T> q = new ConcurrentQueue<T>();
        private object lockObject = new object();

        public int Limit { get; private set; }

        public FixedSizeQueue(int limit) => this.Limit = limit;

        public void Enqueue(T obj)
        {
            q.Enqueue(obj);
            lock (lockObject)
            {
                T overflow;
                while (q.Count > Limit && q.TryDequeue(out overflow)) ;
            }
        }

        public void Dequeue(T obj) => this.q.TryDequeue(out T resultObj);

        public void Clear() => q.Clear();
        public IEnumerator<T> GetEnumerator() => this.q.GetEnumerator();
        public int Count() => q.Count;
        public T ElementAtOrDefault(int index) => q.ElementAtOrDefault<T>(index);
    }
}
