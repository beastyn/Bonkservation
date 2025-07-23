using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;
using Managers;

namespace Agent
{
    public class BonkReaction : Action
    {
        protected override bool NeedSceneRefs => false;

        public float FlyAwayForceY = 1000f;
        public float FlyAwayForceXZ = 1000f;
        public float rigidbodyVelocityThreshold = 0.1f;
        public float upAngled = 15f;
        public float upNormalized = 0.1f;
      
        NavMeshAgent navMeshAgent;
        Rigidbody rigidbody;
   
        Transform maneSan = null;
        bool skipFrame = true;

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
            this.rigidbody = this.AgentObject.GetComponent<Rigidbody>();

            if (this.navMeshAgent == null || this.rigidbody == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.navMeshAgent.enabled = false;
            this.rigidbody.isKinematic = false;
            var indicatorsManager = this.AgentObject.GetComponent<IndicatorsManager>();
            this.maneSan = indicatorsManager?.GetManeSan();
            if (this.maneSan == null)
            {
                this.ReferenceMissing = true;
                return;
            }

            Quaternion rotation = Quaternion.AngleAxis(this.upAngled, Vector3.right); // Z axis is (0,0,1)

            var dirNormal = (this.AgentObject.transform.position - this.maneSan.position).normalized;
            //Vector3 rotatedVector = rotation * new Vector3(dir.x, 0f, dir.y).normalized;
            var upward = Vector3.up * this.upNormalized;
            this.rigidbody.AddForce(upward * this.FlyAwayForceY, ForceMode.Impulse);
            this.rigidbody.AddForce(new Vector3(dirNormal.x, 0f, dirNormal.z) * this.FlyAwayForceXZ, ForceMode.Impulse);
           

        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing || this.maneSan == null)
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
            this.navMeshAgent.enabled = true;
            this.rigidbody.isKinematic = true;
            this.skipFrame = true;
        }

        protected override void OnInterrupt() => this.OnDisable();
    }
}
