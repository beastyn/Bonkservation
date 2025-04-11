using Unity.Entities;
using UnityEngine;

namespace Agent
{
    public struct BoidSchool : IComponentData
    {
        public Entity Prefab;
        public float InitialRadius;
        public int Count;
    }
}