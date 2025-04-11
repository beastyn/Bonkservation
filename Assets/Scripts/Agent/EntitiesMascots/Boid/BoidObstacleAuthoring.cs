using Unity.Entities;
using UnityEngine;

namespace Agent
{
    public class BoidObstacleAuthoring : MonoBehaviour
    {
        public class Baker : Baker<BoidObstacleAuthoring>
        {
            public override void Bake(BoidObstacleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new BoidObstacle());
            }
        }

    }
}
