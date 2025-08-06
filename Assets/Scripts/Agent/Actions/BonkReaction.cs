using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;
using Managers;

namespace Agent
{
    public class BonkReaction : Action
    {
        protected override bool NeedSceneRefs => false;

        public float rigidbodyVelocityThreshold = 0.1f;        

        bool skipFrame = true;

        AgentManagersAndData agentManagersAndData;

        IndicatorsManager indicatorManager;
        NavMeshAgent navMeshAgent;
        Rigidbody rigidbody;


        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();
            
            this.navMeshAgent = this.agentManagersAndData.NavMeshAgentComponent;
            this.rigidbody = this.agentManagersAndData.MainRigidbodyComponent;
            this.indicatorManager = this.agentManagersAndData.IndicatorsManager;

            if (this.navMeshAgent == null || this.rigidbody == null || this.indicatorManager == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.navMeshAgent.enabled = false;
            this.rigidbody.isKinematic = false;
        }

        protected override NodeState OnUpdate()
        {
            if (this.ReferenceMissing)
                return NodeState.Failure;

            if (this.rigidbody.linearVelocity.magnitude > this.rigidbodyVelocityThreshold || skipFrame)
            {
                this.skipFrame = false;              

                return NodeState.Running;
            }
            return NodeState.Success;
        }

        protected override void OnDisable()
        {
            this.rigidbody.isKinematic = true;
            this.navMeshAgent.enabled = true;
            this.skipFrame = true;
            this.indicatorManager.ResetBonkedIndicator();
            if (!this.navMeshAgent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(this.AgentObject.transform.position, out hit, 5.0f, NavMesh.AllAreas))
                {
                    this.AgentObject.transform.position = hit.position;

                    // Important: disable & re-enable agent if needed
                    this.navMeshAgent.enabled = false;
                    this.navMeshAgent.enabled = true;
                }
            }
        }

        protected override void OnInterrupt() => this.OnDisable();
    }
}
