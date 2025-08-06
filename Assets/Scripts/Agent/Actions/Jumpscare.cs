using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Linq;
using Unity.VisualScripting;

namespace Agent
{
    public class Jumpscare : Action
    {
        protected override bool NeedSceneRefs => false;

        public float rotationSpeed = 50f;
        public float angleThreshold = 5.0f;

        AgentManagersAndData agentManagersAndData;

        NavMeshAgent navMeshAgent;
        Animator animator;

        Transform maneSan = null;
        Quaternion lookRotation;

        bool animationWasLaunched = false;
        bool rotationIsFinished = false;

        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData?.NavMeshAgentComponent;
            this.animator = this.agentManagersAndData?.AnimatorComponent;
            this.animator ??= this.AgentObject.GetComponentInChildren<Animator>();

            this.maneSan = this.agentManagersAndData?.IndicatorsManager.GetManeSan();

            if (this.agentManagersAndData == null || this.navMeshAgent == null || !this.navMeshAgent.isOnNavMesh || this.maneSan == null || this.animator == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;

            Vector3 direction = (this.maneSan.position - this.AgentObject.transform.position).normalized;
            this.lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }

        protected override NodeState OnUpdate()
        {
            if (this.ReferenceMissing)
                return NodeState.Failure;

            if (!this.rotationIsFinished)
                this.AgentObject.transform.rotation = Quaternion.Slerp(this.AgentObject.transform.rotation, this.lookRotation, Time.deltaTime * this.rotationSpeed); //Quaternion.RotateTowards(this.AgentObject.transform.rotation, this.lookRotation, this.rotationSpeed * Time.deltaTime); //Quaternion.Slerp(this.AgentObject.transform.rotation, this.lookRotation, Time.deltaTime * this.rotationSpeed);
            
            var animationIsRunning = this.animator.GetCurrentAnimatorStateInfo(0).IsName(this.GetType().Name);
            if(animationIsRunning) this.animationWasLaunched = true;

            this.rotationIsFinished = Quaternion.Angle(this.AgentObject.transform.rotation, this.lookRotation) < this.angleThreshold;

            if (!this.rotationIsFinished || !this.animationWasLaunched)
            {
                return NodeState.Running;
            }

            return NodeState.Success;

        }

        protected override void OnDisable()
        {
            this.rotationIsFinished = false;
            this.animationWasLaunched = false;
        }

    }
}
