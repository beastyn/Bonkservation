
namespace Pooling
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        public T Prefab;
        public int PoolSize = 20;
        public int ActiveObjectsCount = 0;

        Queue<T> inactiveObjects = new Queue<T>();

        protected virtual void Start()
        {
            // Initialize the object pool
            for (int i = 0; i < PoolSize; i++)
            {
                T obj = Instantiate(Prefab, transform);
                obj.gameObject.SetActive(false);
                inactiveObjects.Enqueue(obj);
            }
        }

        public T GetObject()
        {
            // Check if there's an inactive object in the pool
            if (inactiveObjects.Count > 0)
            {
                T obj = inactiveObjects.Dequeue();
                obj.gameObject.SetActive(true);
                this.ActiveObjectsCount++;
                return obj;
            }

            // If there are no inactive objects, create a new one
            T newObj = Instantiate(Prefab, transform);
            newObj.gameObject.SetActive(true);
            this.ActiveObjectsCount++;
            return newObj;
        }

        public void ReturnObject(T obj)
        {
            // Deactivate the object and return it to the pool
            obj.gameObject.SetActive(false);
            inactiveObjects.Enqueue(obj);
            this.ActiveObjectsCount--;
        }
    }
}
