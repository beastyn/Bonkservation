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

        AgentManagersAndData agentManagersAndData;

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

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData?.NavMeshAgentComponent;  
            
            var groundTransform = this.SceneRefs.GetRef<Transform>(this.transformIndex);
            this.navMeshSurface = groundTransform.GetComponent<NavMeshSurface>();

            if (this.agentManagersAndData == null || this.navMeshAgent == null || this.navMeshSurface==null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
           
            if (this.ReferenceMissing || !this.navMeshAgent.isOnNavMesh)
                return;

            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.MinDistance;
            this.currDestination = Utils.UtilsTools.GetRandomPointInNavMeshVolume(this.navMeshSurface, this.MaxDistance);
            this.navMeshAgent.destination = this.currDestination;
        }

        protected override NodeState OnUpdate()
        {
            if (this.ReferenceMissing || !this.navMeshAgent.isOnNavMesh || this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
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

        protected override void OnInterrupt() => this.OnDisable();
    }
}
