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

        NavMeshAgent navMeshAgent;
        SOEnergy agentEnergy;
        Damageable damageable;

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
            this.agentEnergy = this.AgentObject.GetComponent<AgentSOHolder>()?.AgentEnergy;
            this.damageable = this.AgentObject.GetComponent<Damageable>();

            if (this.navMeshAgent == null || this.agentEnergy == null || this.damageable == null)
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
