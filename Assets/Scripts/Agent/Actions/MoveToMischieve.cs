using UnityEngine;
using UnityEngine.AI;
using BrainDesigner.Scripts.Runtime;
using Managers;

namespace Agent
{
    public class MoveToMischieve : Action
    {
        public float MinDistance = 1f;
        public float InitialSpeed = 5f;

        [SerializeField] int transformIndex;
        protected override bool NeedSceneRefs => false;

        NavMeshAgent navMeshAgent;
        CollisionDetector collisionDetector;

        Transform parentForMischives;
        Transform currentMischieve;
        Vector3 currDestination = Vector3.zero;

        protected override void RegisterDropdowns()
        {
            AddDropdown("Parent of Mischieves", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
                newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
            this.collisionDetector = this.AgentObject.GetComponent<CollisionDetector>();
            this.parentForMischives = this.SceneRefs.GetRef<Transform>(this.transformIndex);

            if (this.navMeshAgent == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = ManagersSOHolder.SODifficultySettings.GetSpeedWithModification(this.InitialSpeed);
            this.navMeshAgent.stoppingDistance = this.MinDistance;
            this.currDestination = this.GetRandomMischieve();
        }

        protected override NodeState OnUpdate()
        {
            this.navMeshAgent.destination = this.currDestination;

            if (this.ReferenceMissing || this.collisionDetector == null || this.currDestination == Vector3.zero || this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
                return NodeState.Failure;

            /*float sqrDistanceToTarget = (this.agentTransform.position - this.currDestination).sqrMagnitude;
            float stoppingDistance = this.navMeshAgent.stoppingDistance;
            float sqrStoppingDistance = stoppingDistance * stoppingDistance;*/

            if (this.collisionDetector.MischieveCollided && this.currentMischieve == this.collisionDetector.MischieveCollidedObject?.transform)
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

        public Vector3 GetRandomMischieve()
        {
            this.currentMischieve = this.parentForMischives.GetChild(Random.Range(0, this.parentForMischives.childCount));
            return this.currentMischieve.position; // Return the valid point inside the NavMesh volume
        }
    }
}
