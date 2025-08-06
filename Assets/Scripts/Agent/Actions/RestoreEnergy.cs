using UnityEngine;
using UnityEngine.AI;
using BrainDesigner.Scripts.Runtime;
using Gameplay;
using Unity.VisualScripting;

namespace Agent
{
    public class RestoreEnergy : Action
    {
        protected override bool NeedSceneRefs => false;

        AgentManagersAndData agentManagersAndData;

        NavMeshAgent navMeshAgent;
        SOEnergy agentEnergy;
        Damageable damageable;

        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData?.NavMeshAgentComponent;
            this.agentEnergy = this.agentManagersAndData?.AgentEnergy;
            this.damageable = this.agentManagersAndData?.DamageableComponent;

            if (this.agentManagersAndData == null || this.navMeshAgent == null || !this.navMeshAgent.isOnNavMesh || this.agentEnergy == null || this.damageable == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.agentEnergy.SetEnergyRestore(true);
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing)
                return NodeState.Failure;

            if (this.agentEnergy.IsRegenerating)
            {
                this.agentEnergy.FullRestoreEnergy(this.agentEnergy.RestoreEnergyValue);
                return NodeState.Running;
            }

            return NodeState.Success;
        }
    }
}
