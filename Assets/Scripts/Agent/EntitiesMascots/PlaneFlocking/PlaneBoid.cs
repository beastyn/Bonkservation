using Unity.Entities;
using UnityEngine;

namespace Agent
{
    public struct PlaneBoid : ISharedComponentData
    {
        public float CellRadius;
        public float SeparationWeight;
        public float AlignmentWeight;
        public float TargetWeight;
        public float ObstacleAversionDistance;
        public float MoveSpeed;
    }
}
