using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using Gameplay;
using Managers;
using UnityEngine.AI;

namespace Agent
{
    public class DoJob : Action
    {
        public float MinJobTime = 1f;
        public float MaxJobTime = 5f;
        protected override bool NeedSceneRefs => false;

        AgentManagersAndData agentManagersAndData;

        NavMeshAgent navMeshAgent;
        SOMischieve agentMischieve;
        float goalMischieveTime = 1f;
        float startTime;
        float currentTime;

        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData?.NavMeshAgentComponent;
            this.agentMischieve = this.agentManagersAndData?.AgentMischieve;

            if (this.agentManagersAndData == null || this.navMeshAgent == null || this.agentMischieve == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.goalMischieveTime = this.GetRandomJobTime();

            this.startTime = Time.realtimeSinceStartup;
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing)
                return NodeState.Failure;

            this.currentTime = Time.realtimeSinceStartup;

            if (this.currentTime - this.startTime < this.goalMischieveTime)
            {
                return NodeState.Running;
            }

            return NodeState.Success;
        }

        protected override void OnDisable()
        {
            this.startTime = 0f;
            this.currentTime = 0f;
        }

        protected override void OnInterrupt() => this.OnDisable();

        public float GetRandomJobTime() => Random.Range(this.MinJobTime, this.MaxJobTime);
    }
}
