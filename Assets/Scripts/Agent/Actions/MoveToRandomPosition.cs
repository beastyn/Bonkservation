using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using BrainDesigner.Scripts.Runtime;

namespace Agent
{
    public class MoveToRandomPosition : Action
    {       
        public float MaxDistance = 300.0f;
        public float MinDistance = 1f;
        public float InitialSpeed = 5f;
        protected override bool NeedSceneRefs => true;

        [SerializeField] int transformIndex;

        NavMeshAgent navMeshAgent;
        NavMeshSurface navMeshSurface;

        Vector3 currDestination = Vector3.zero;

        protected override void RegisterDropdowns()
        {
            base.RegisterDropdowns();
            AddDropdown("NavMesh Surface Transform", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
              newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();  
            
            var groundTransform = this.SceneRefs.GetRef<Transform>(this.transformIndex);
            this.navMeshSurface = groundTransform.GetComponent<NavMeshSurface>();

            if (this.navMeshAgent == null && this.navMeshSurface==null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
           
            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.MinDistance;
            this.currDestination = this.GetRandomPointInNavMeshVolume();
        }

        protected override NodeState OnUpdate()
        {
            this.navMeshAgent.destination = this.currDestination;

            if (this.ReferenceMissing || this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                return NodeState.Failure;
            }
            return this.navMeshAgent.remainingDistance <= this.navMeshAgent.stoppingDistance ? NodeState.Success : NodeState.Running;
        }

        protected override void OnDisable()
        {
            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.MinDistance;
        }

        public Vector3 GetRandomPointInNavMeshVolume()
        {
            // Get the bounds of the NavMesh surface
            Bounds navMeshBounds = this.navMeshSurface.navMeshData.sourceBounds;

            Vector3 randomPoint;
            NavMeshHit hit;

            // Try to generate a point inside the volume bounds
            do
            {
                randomPoint = new Vector3(
                    Random.Range(navMeshBounds.min.x, navMeshBounds.max.x), // Random x
                    navMeshBounds.center.y,                                // Fixed Y level
                    Random.Range(navMeshBounds.min.z, navMeshBounds.max.z)  // Random z
                );

                // Check if the random point is on the NavMesh within maxDistance
            } while (!NavMesh.SamplePosition(randomPoint, out hit, this.MaxDistance, NavMesh.AllAreas));

            return hit.position; // Return the valid point inside the NavMesh volume
        }       
    }
}
