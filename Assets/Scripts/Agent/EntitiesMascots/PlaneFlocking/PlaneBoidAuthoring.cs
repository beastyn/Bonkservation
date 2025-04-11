using Unity.Entities;
using UnityEngine;

namespace Agent
{
    public class PlaneBoidAuthoring : MonoBehaviour
    {
        public float CellRadius = 8.0f;
        public float SeparationWeight = 1.0f;
        public float AlignmentWeight = 1.0f;
        public float TargetWeight = 2.0f;
        public float ObstacleAversionDistance = 30.0f;
        public float MoveSpeed = 25.0f;

        class Baker : Baker<PlaneBoidAuthoring>
        {
            public override void Bake(PlaneBoidAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable | TransformUsageFlags.WorldSpace);
                AddSharedComponent(entity, new PlaneBoid
                {
                    CellRadius = authoring.CellRadius,
                    SeparationWeight = authoring.SeparationWeight,
                    AlignmentWeight = authoring.AlignmentWeight,
                    TargetWeight = authoring.TargetWeight,
                    ObstacleAversionDistance = authoring.ObstacleAversionDistance,
                    MoveSpeed = authoring.MoveSpeed
                });
            }
        }
    }
}
