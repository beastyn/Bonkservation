using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts.Utils
{
    [Serializable]
    public abstract class Set<T> : Named where T:Named
    {
        [SerializeReference] internal List<T> list = new();
        internal Action ListChangedEvent;

        
        internal void AddSetElement(T value)
        {
            this.list.Add(value);
            this.ListChangedEvent?.Invoke();
        }
        internal void RemoveSetElementAt(int index)
        {
            this.list.RemoveAt(index);
            this.ListChangedEvent?.Invoke();
        }


        internal bool TryGetElementByName(string name, out T foundObject)
        {
            foreach (var obj in list)
            {
                if (obj.Name == name)
                {
                    foundObject = obj;
                    return true;
                }

            }
            Debug.Log($"Element {name} was not found.");
            foundObject = default;
            return false;
        }
    }
}