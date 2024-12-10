using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;

namespace Agent
{
    public class Mischieve : Action
    { 
        public float MinIdleTime = 1f;
        public float MaxIdleTime = 5f;
        protected override bool NeedSceneRefs => false;
       

        NavMeshAgent navMeshAgent;
        float goalIdleTime = 1f;
        float startTime;
        float currentTime;

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();

            if (this.navMeshAgent == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.goalIdleTime = this.GetRandomIdleTime();

            this.startTime = Time.realtimeSinceStartup;
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing)
                return NodeState.Failure;

            this.currentTime = Time.realtimeSinceStartup;

            if (this.currentTime - this.startTime < this.goalIdleTime)
                return NodeState.Running;

            return NodeState.Success;
        }

        protected override void OnDisable()
        {
            this.startTime = 0f;
            this.currentTime = 0f;
        }

        protected override void OnInterrupt() => this.OnDisable();

        public float GetRandomIdleTime() => Random.Range(this.MinIdleTime, this.MaxIdleTime); // Return the valid point inside the NavMesh volume
    }
}