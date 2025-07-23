using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts.Runtime
{
    public abstract class SceneReferences : MonoBehaviour
    {
        readonly Dictionary<Type, object> lists = new();

        /// <summary>
        /// Register lists for different component types using the "AddList" method.
        /// </summary>
        protected abstract void RegisterCustomLists();

        /// <summary>
        /// Adds a list of components of type T.
        /// </summary>
        /// <typeparam name="T">List type to add.</typeparam>
        /// <param name="list">List of components of type T.</param>
        protected void AddList<T>(List<T> list)
        {
            this.lists.Add(typeof(T), list);
        }

        void Awake()
        {
            Initialize();
        }

        internal void Initialize()
        {
            this.lists.Clear();
            RegisterCustomLists();
        }

        /// <summary>
        /// Retrieves a component of type T at the specified index.
        /// </summary>
        /// <typeparam name="T">Component type to search for.</typeparam>
        /// <param name="index">The index of the desired component.</param>
        /// <returns>Component at index if found, otherwise default value for type T.</returns>
        public T GetRef<T>(int index)
        {
            Type type = typeof(T);

            if (this.lists.TryGetValue(type, out object list) && list is IList<T> typedList)
                if (typedList.Count > index)
                    return typedList[index];

            return default;
        }

        /// <summary>
        /// Gets the first component of type T on a GameObject with the given name.
        /// </summary>
        /// <typeparam name="T">Component type to search for.</typeparam>
        /// <param name="gameObjectName">Name of the target GameObject.</param>
        /// <returns>First instance of T found, or null if not found.</returns>
        public T GetRef<T>(string gameObjectName) where T : Component
        {
            Type type = typeof(T);

            if (this.lists.TryGetValue(type, out object list) && list is IList<T> typedList)
            {
                T result = typedList.FirstOrDefault(component => component.gameObject.name == gameObjectName);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Gets the first component of type T that satisfies the provided custom comparison function.
        /// </summary>
        /// <typeparam name="T">Component type to search for.</typeparam>
        /// <param name="customComparison">Custom comparison function.</param>
        /// <returns>First instance of T found, or null if not found.</returns>
        public T GetRef<T>(Func<T, bool> customComparison) where T : Component
        {
            Type type = typeof(T);

            if (this.lists.TryGetValue(type, out object list) && list is IList<T> typedList)
            {
                T result = typedList.FirstOrDefault(customComparison);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the list of components of type T.
        /// </summary>
        /// <typeparam name="T">List type to search for.</typeparam>
        /// <returns>List of components of type T if found, or null if not found.</returns>
        public List<T> GetListOfType<T>()
        {
            if (this.lists.TryGetValue(typeof(T), out object list) && list is List<T> typedList)
                return typedList;

            return null;
        }
    }
}