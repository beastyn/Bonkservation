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

        float currIdleTime = 0f;
        float goalIdleTime = 1f;

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
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing)
                return NodeState.Failure;

            if (this.currIdleTime < this.goalIdleTime)
            {
                this.currIdleTime += Time.deltaTime;
                return NodeState.Running;
            }

            return NodeState.Success;
        }

        protected override void OnDisable()
        {
            this.currIdleTime = 0f;
        }

        public float GetRandomIdleTime() => Random.Range(this.MinIdleTime, this.MaxIdleTime); // Return the valid point inside the NavMesh volume
    }
}