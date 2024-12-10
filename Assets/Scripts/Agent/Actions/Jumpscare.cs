using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace Agent
{
    public class Jumpscare : Action
    {
        /*float appearTime = 0.5f;
        float hideTime = 0.5f;
        float moveBy = 4f;

        bool runSequence = true;
        bool canHide = true;
        bool isHideSequence = false;*/
        protected override bool NeedSceneRefs => true;

        [SerializeField] int transformIndex;

        Transform jumpObjectsParent;
        int jumpObjectsCount;
        Vector3 positionToJump;
        NavMeshAgent navMeshAgent;

        protected override void RegisterDropdowns()
        {
            base.RegisterDropdowns();
            AddDropdown("Parent for Jumscare hides", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
              newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
            this.jumpObjectsParent = this.SceneRefs.GetRef<Transform>(this.transformIndex);

            if (this.jumpObjectsParent == null && this.navMeshAgent == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.jumpObjectsCount = this.jumpObjectsParent.childCount;
            var objectToJump = this.jumpObjectsParent.GetChild(Random.Range(0, this.jumpObjectsCount));
            this.positionToJump = new Vector3(objectToJump.position.x, this.AgentObject.transform.position.y, objectToJump.position.z);
        }

        protected override NodeState OnUpdate()
        {
            if (this.jumpObjectsCount <= 0)
                return NodeState.Failure;

            if (this.AgentObject.transform.position != this.positionToJump)
            {
                this.AgentObject.transform.localScale = Vector3.zero;
                this.AgentObject.transform.position = this.positionToJump;
                this.AgentObject.transform.localScale = Vector3.one;
                return NodeState.Running;
            }

            return NodeState.Success;

        }

        protected override void OnDisable()
        {
            
        }

    }
}
