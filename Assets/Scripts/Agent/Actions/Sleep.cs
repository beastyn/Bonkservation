using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;
using Managers;
using Gameplay;

namespace Agent
{
    public class Sleep : Action
    {
        public float RestoreEnergySpeed = 100f;

        protected override bool NeedSceneRefs => false;

        NavMeshAgent navMeshAgent;
        SOEnergy agentEnergy;
        Damageable damageable;

        float startTime;
        float currentTime;
        float goalIdleTime = 1f;

        bool requestWakeUp = false;

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
            this.agentEnergy = this.AgentObject.GetComponent<AgentSOHolder>()?.AgentEnergy;
            this.damageable = this.AgentObject.GetComponent<Damageable>();

            if (this.navMeshAgent == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.agentEnergy.SetEnergyRestore(true);

            TimeManager.SunriseEvent += OnSunriseEvent;
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing || (this.requestWakeUp && this.agentEnergy.CurrentValue != this.agentEnergy.MaxValue))
                return NodeState.Failure;


            this.currentTime = Time.realtimeSinceStartup;

            if (!this.requestWakeUp && !TimeManager.IsSunRisen)
            {
                if (this.agentEnergy.IsRegenerating)
                    this.agentEnergy.FullRestoreEnergy(this.RestoreEnergySpeed);
                return NodeState.Running;
            }

            return NodeState.Success;
        }

        protected override void OnDisable()
        {
            this.startTime = 0f;
            this.currentTime = 0f;
            this.requestWakeUp = false;
            this.agentEnergy.SetEnergyRestore(false);
            TimeManager.SunriseEvent -= OnSunriseEvent;
        }

        void OnSunriseEvent() => this.requestWakeUp = true;

        protected override void OnInterrupt() => this.OnDisable();
    }
}
