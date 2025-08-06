using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;
using Managers;
using Gameplay;

namespace Agent
{
    public class Mischieve : Action
    { 
        public float MinMischieveTime = 1f;
        public float MaxMischieveTime = 5f;
        public int BaseMischieveProbobality = 10;
        protected override bool NeedSceneRefs => false;

        AgentManagersAndData agentManagersAndData;

        NavMeshAgent navMeshAgent;
        AgentManagersAndData soHolder;
        SOMischieve agentMischieve;
        SOAudioCollection audioCollection;
        Damageable damageable;
        EffectsController effectController;
        AudioSource audioSource;
        float goalMischieveTime = 1f;
        float startTime;
        float currentTime;
        bool requestMischieve = false;

        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData = this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData?.NavMeshAgentComponent;            
            this.agentMischieve = this.agentManagersAndData?.AgentMischieve;
            this.audioCollection = this.agentManagersAndData?.AudioCollections;
            this.damageable = this.agentManagersAndData?.DamageableComponent;
            this.audioSource = this.agentManagersAndData?.AudioSourceComponent;

            if (this.agentManagersAndData == null || this.navMeshAgent == null || !this.navMeshAgent.isOnNavMesh || this.agentMischieve == null || this.damageable == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {

            if (this.ReferenceMissing)
                return;

            this.navMeshAgent.speed = 0f;
            this.goalMischieveTime = this.GetRandomMischieveTime();

            this.startTime = Time.realtimeSinceStartup;
            this.damageable.SetProtection(false);
            this.agentManagersAndData.EffectsController.SwitchMischieveEffect(true);


            this.requestMischieve = ManagersSOHolder.SODifficultySettings.ShouldLaunchActionByDifficulty(this.BaseMischieveProbobality);
            if (requestMischieve) AudioMixerManager.PlayRundomCollectionClip(this.audioSource, this.audioCollection, AudioCollectionName.JobMischieves);
        }

        protected override NodeState OnUpdate()
        {

            if (this.ReferenceMissing || !this.requestMischieve)
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
            this.damageable.SetProtection(true);
            this.agentManagersAndData.EffectsController.SwitchMischieveEffect(false);
        }

        protected override void OnInterrupt() => this.OnDisable();

        public float GetRandomMischieveTime() => ManagersSOHolder.SODifficultySettings.GetMischieveTimeWithModification(Random.Range(this.MaxMischieveTime, this.MaxMischieveTime));
    }
}