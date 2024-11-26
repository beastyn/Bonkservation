
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

namespace BrainDesigner.Scripts
{
    using Runtime;
    public class BehaviourManager : MonoBehaviour
    {
        [SerializeField] TaskedAgentData behaviourData;

        [SerializeField] internal float tickRateEvaluation = 0.1f;
        [SerializeField] internal bool useUpdateAsTickRateForEvaluation;
        [SerializeField] internal float tickRateExecution = 0.1f;
        [SerializeField] internal bool useUpdateAsTickRateForExecution;
        [SerializeField] internal float tickRateLog = 1f;
/*
        [Header("MoveTo")]
        [SerializeField] float roamMaxDistance = 300.0f;
        [SerializeField] float roamMinDistance = 1f;
        [SerializeField] float roamSpeed = 5f;
        //[SerializeField] NavMeshSurface navMeshSruface;

        [Header("Idle")]
        [SerializeField] float minIdleTime = 1f;
        [SerializeField] float maxIdleTime = 5f;

        [Header("Mischieves")]
        [SerializeField] Transform[] mischieves;

        [Header("Jumpscare")]
        [SerializeField] Transform jumpObjecstParent; */

        BehaviourSet currenBehaviourSet = new();
        Behaviour currentBehaviour = null;

        void Awake()
        {
            /*//Roaming Behavior
            var roamBehaviour = new Behaviour();
            roamBehaviour.name = "RoamRandom";
            roamBehaviour.InitializeNode(this.gameObject);

            var moveToRandomPosition = new MoveToRandomPosition(this.navMeshSruface, roamMaxDistance, roamMinDistance, roamSpeed);
            moveToRandomPosition.InitializeNode(this.gameObject);

            var idle = new Idle(this.minIdleTime, this.maxIdleTime);
            idle.InitializeNode(this.gameObject);

            roamBehaviour.children.Add(moveToRandomPosition);
            roamBehaviour.children.Add(idle);

            //Mischieve Behavior
            var mischieveBehaviour = new Behaviour();
            mischieveBehaviour.name = "Mischieve";
            mischieveBehaviour.InitializeNode(this.gameObject);

            var moveToTransform = new MoveToMischieve(this.mischieves, roamMinDistance, roamSpeed);
            moveToTransform.InitializeNode(this.gameObject);

            var mischieve = new Mischieve(this.minIdleTime, this.maxIdleTime);
            mischieve.InitializeNode(this.gameObject);

            mischieveBehaviour.children.Add(moveToTransform);
            mischieveBehaviour.children.Add(mischieve);

            //Jumpscare Behaviour
            var jumpscareBehaviour = new Behaviour();
            jumpscareBehaviour.name = "Run from Mane-san";
            jumpscareBehaviour.InitializeNode(this.gameObject);

            var jumpAction = new Jumpscare(this.jumpObjecstParent);
            jumpAction.InitializeNode(this.gameObject);

            var jumpPoke = new Idle(1f, 3f);
            jumpPoke.InitializeNode(this.gameObject);

            jumpscareBehaviour.children.Add(jumpAction);
            jumpscareBehaviour.children.Add(jumpPoke);

            this.currenBehaviourSet.List.Add(roamBehaviour);
            this.currenBehaviourSet.List.Add(mischieveBehaviour);
            this.currenBehaviourSet.List.Add(jumpscareBehaviour);

            this.behaviourData.RememberBehaviourSet(this.currenBehaviourSet);*/
        }

        void Update()
        {
            this.Evaluate();
        }

        void Evaluate()
        {
           /* List<Behaviour> eligableBehaviuors = new();
            foreach (var behaviour in this.currenBehaviourSet.list)
            {
                if (behaviour.IsValidToStart()) eligableBehaviuors.Add(behaviour); 
            }

            if (this.currentBehaviour == null || this.currentBehaviour.behaviourSequence.nodeState != Node.NodeState.Running)
            {
                var activeIndex = UnityEngine.Random.Range(0, eligableBehaviuors.Count());
                this.currentBehaviour = eligableBehaviuors[2];
            }

            this.currentBehaviour.behaviourSequence.Update();*/
        }


        /*    internal void Initialize()
            {
                sceneReferences = GameObject.Find(sceneReferencesObjName)?.GetComponent<SceneReferences>();
            }*/
    }
}
