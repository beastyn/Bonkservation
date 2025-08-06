using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;

namespace Agent
{
    public class MoveToObject : Action
    {
        public float MinDistance = 1f;
        public float InitialSpeed = 5f;

        [SerializeField] int transformIndex;
        protected override bool NeedSceneRefs => true;

        AgentManagersAndData agentManagersAndData;

        NavMeshAgent navMeshAgent;

        Transform targetObject;
        Transform currentMischieve;
        Vector3 currDestination = Vector3.zero;

        protected override void RegisterDropdowns()
        {
            AddDropdown("Target object", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
                newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData?.NavMeshAgentComponent;
            this.targetObject = this.SceneRefs.GetRef<Transform>(this.transformIndex);

            if (this.agentManagersAndData == null || this.navMeshAgent == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing || !this.navMeshAgent.isOnNavMesh)
                return;

            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.MinDistance;
            this.currDestination = this.targetObject.position;
            this.navMeshAgent.destination = this.currDestination;
        }

        protected override NodeState OnUpdate()
        {
            if (this.ReferenceMissing || !this.navMeshAgent.isOnNavMesh || this.currDestination == Vector3.zero || this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
                return NodeState.Failure;

            /*float sqrDistanceToTarget = (this.agentTransform.position - this.currDestination).sqrMagnitude;
            float stoppingDistance = this.navMeshAgent.stoppingDistance;
            float sqrStoppingDistance = stoppingDistance * stoppingDistance;*/

            if ((this.navMeshAgent.transform.position - this.currDestination).sqrMagnitude <= this.MinDistance)
                return NodeState.Success;
            else
                return NodeState.Running;


            //return this.collisionDetector.MischieveCollided && this.currentMischieve == this.collisionDetector.CollidedObject  ? NodeState.Success : NodeState.Running;//
        }

        protected override void OnDisable()
        {
            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.MinDistance;
            this.currentMischieve = null;
            this.currDestination = Vector3.zero;
        }

        protected override void OnInterrupt() => this.OnDisable();
    }
}
