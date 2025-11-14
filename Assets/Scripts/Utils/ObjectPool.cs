using System;
using System.Collections.Generic;
using UnityEngine;

namespace KaiAlphabetAdventure.Utils
{
    /// <summary>
    /// Generic object pooling system for performance optimization.
    /// Supports: REQ-2.4.3
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private Queue<T> pool = new Queue<T>();
        private HashSet<T> activeObjects = new HashSet<T>();

        private Func<T> createFunc;
        private Action<T> onGet;
        private Action<T> onRelease;
        private int initialSize;

        public int ActiveCount => activeObjects.Count;
        public int AvailableCount => pool.Count;
        public int TotalCount => ActiveCount + AvailableCount;

        public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, int initialSize = 10)
        {
            this.createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            this.onGet = onGet;
            this.onRelease = onRelease;
            this.initialSize = initialSize;

            // Pre-populate pool
            for (int i = 0; i < initialSize; i++)
            {
                T obj = createFunc();
                pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                obj = createFunc();
                Debug.Log($"ObjectPool: Created new object. Total: {TotalCount}");
            }

            activeObjects.Add(obj);
            onGet?.Invoke(obj);

            return obj;
        }

        public void Release(T obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("ObjectPool: Tried to release null object");
                return;
            }

            if (!activeObjects.Contains(obj))
            {
                Debug.LogWarning("ObjectPool: Tried to release object that wasn't active");
                return;
            }

            activeObjects.Remove(obj);
            onRelease?.Invoke(obj);
            pool.Enqueue(obj);
        }

        public void Clear()
        {
            foreach (T obj in activeObjects)
            {
                if (obj != null && obj.gameObject != null)
                {
                    UnityEngine.Object.Destroy(obj.gameObject);
                }
            }

            foreach (T obj in pool)
            {
                if (obj != null && obj.gameObject != null)
                {
                    UnityEngine.Object.Destroy(obj.gameObject);
                }
            }

            activeObjects.Clear();
            pool.Clear();
        }
    }
}
