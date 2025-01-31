using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;
using Managers;

namespace Agent
{
    public class Mischieve : Action
    { 
        public float MinMischieveTime = 1f;
        public float MaxMischieveTime = 5f;
        protected override bool NeedSceneRefs => false;
       

        NavMeshAgent navMeshAgent;
        SOMischieve agentMischieve;
        float goalMischieveTime = 1f;
        float startTime;
        float currentTime;

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
            this.agentMischieve = this.AgentObject.GetComponent<AgentSOHolder>()?.AgentMischieve;

            if (this.navMeshAgent == null && this.agentMischieve == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.goalMischieveTime = this.GetRandomMischieveTime();

            this.startTime = Time.realtimeSinceStartup;
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing)
                return NodeState.Failure;

            this.currentTime = Time.realtimeSinceStartup;

            if (this.currentTime - this.startTime < this.goalMischieveTime)
            {
                this.agentMischieve.UpdateMischieveFill(this.agentMischieve.Speed * Time.deltaTime);
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

        public float GetRandomMischieveTime() => Random.Range(this.MinMischieveTime, this.MaxMischieveTime);
    }
}