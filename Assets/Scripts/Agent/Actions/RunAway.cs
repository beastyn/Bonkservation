using UnityEngine;
using UnityEngine.AI;
using BrainDesigner.Scripts.Runtime;
using Unity.AI.Navigation;

namespace Agent
{
    public class RunAway : Action
    {
        public float MinDistance = 0.1f;
        public float MinLookAhead = 1f;
        public float RunDistance = 50f;
        public float PointChechDistance = 10.0f;
        public float SearchAngle = 90f;
        public float InitialSpeed = 50f;
        protected override bool NeedSceneRefs => true;
        [SerializeField] int transformIndex;

        AgentManagersAndData agentManagersAndData;

        NavMeshAgent navMeshAgent;
        NavMeshSurface navMeshSurface;
        Transform maneSan = null;
        Vector3 randomRunDestination = Vector3.zero;
        bool destinationExists = false;

        protected override void RegisterDropdowns()
        {
            base.RegisterDropdowns();
            AddDropdown("NavMesh Surface Transform", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
              newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData?.NavMeshAgentComponent;

            var groundTransform = this.SceneRefs.GetRef<Transform>(this.transformIndex);
            this.navMeshSurface = groundTransform.GetComponent<NavMeshSurface>();

            this.maneSan = this.agentManagersAndData?.IndicatorsManager.GetManeSan();

            if (this.agentManagersAndData == null || this.navMeshAgent == null || this.maneSan == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
            if (this.ReferenceMissing || !this.navMeshAgent.isOnNavMesh)
                return;

            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.MinDistance;

            var direction = new Vector3(this.AgentObject.transform.position.x - this.maneSan.position.x, 0.0f, this.AgentObject.transform.position.z - this.maneSan.position.z);            
            this.destinationExists = this.RandomNavMeshPoint(this.AgentObject.transform.position, direction.normalized, this.MinLookAhead, this.RunDistance, out this.randomRunDestination);
            if(this.destinationExists)
                this.navMeshAgent.destination = this.randomRunDestination;
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing || !this.navMeshAgent.isOnNavMesh || !this.destinationExists || this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
                return NodeState.Failure;


            

            return !this.navMeshAgent.pathPending && this.navMeshAgent.remainingDistance <= this.navMeshAgent.stoppingDistance ? NodeState.Success : NodeState.Running;
        }

        protected override void OnDisable()
        {
            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.MinDistance;
        }

        protected override void OnInterrupt() => this.OnDisable();

        bool RandomNavMeshPoint(Vector3 center, Vector3 direction, float lookAhead, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                var randomAngle = Random.Range(this.SearchAngle, -this.SearchAngle);
                var randomizedDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * direction;

                var maxDistance = this.GetMaxDistanceWithinBounds(center, randomizedDirection, this.RunDistance, this.navMeshSurface.navMeshData.sourceBounds);
                Vector3 randomPoint = center + randomizedDirection * Random.Range(lookAhead, maxDistance);
                randomPoint.y = this.AgentObject.transform.position.y;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, this.PointChechDistance, NavMesh.AllAreas))
                {
                    Vector3 ret = hit.position;
                    Vector3 pathDir = center - ret;
                    ret += pathDir.normalized * (this.navMeshAgent.radius);
                    result = ret;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        float GetMaxDistanceWithinBounds(Vector3 origin, Vector3 direction, float maxDistance, Bounds bounds)
        {
            // Normalize direction
            direction.Normalize();

            // Calculate inverse direction to avoid division by zero
            Vector3 inverseDirection = new Vector3(
                1.0f / (direction.x != 0 ? direction.x : Mathf.Epsilon),
                1.0f / (direction.y != 0 ? direction.y : Mathf.Epsilon),
                1.0f / (direction.z != 0 ? direction.z : Mathf.Epsilon)
            );

            // Calculate tMin and tMax for each axis
            float tMinX = (bounds.min.x - origin.x) * inverseDirection.x;
            float tMaxX = (bounds.max.x - origin.x) * inverseDirection.x;
            if (tMinX > tMaxX) Swap(ref tMinX, ref tMaxX);

            float tMinZ = (bounds.min.z - origin.z) * inverseDirection.z;
            float tMaxZ = (bounds.max.z - origin.z) * inverseDirection.z;
            if (tMinZ > tMaxZ) Swap(ref tMinZ, ref tMaxZ);

            // Find the largest minimum t and smallest maximum t
            float tMin = Mathf.Max(tMinX, tMinZ, 0.0f); // Ensure we only look forward (t >= 0)
            float tMax = Mathf.Min(tMaxX, tMaxZ, maxDistance);

            // If there's no intersection, return 0
            if (tMin > tMax)
                return 0.0f;

            // The maximum safe distance is tMax
            return tMax;
        }

        /// <summary>
        /// Swaps two values (used for tMin and tMax).
        /// </summary>
        private void Swap(ref float a, ref float b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

    }
}

