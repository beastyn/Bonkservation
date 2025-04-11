using Agent;
using Unity.Entities;
using UnityEngine;

namespace Agent
{
    public class BoidTargetAuthoring : MonoBehaviour
    {
        class Baker : Baker<BoidTargetAuthoring>
        {
            public override void Bake(BoidTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent(entity, new BoidTarget());
            }
        }
    }
}
