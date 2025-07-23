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
       

        NavMeshAgent navMeshAgent;
        AgentSOHolder soHolder;
        SOMischieve agentMischieve;
        SOAudioCollection audioCollection;
        Damageable damageable;
        AudioSource audioSource;
        float goalMischieveTime = 1f;
        float startTime;
        float currentTime;
        bool requestMischieve = false;

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
            this.soHolder = this.AgentObject.GetComponent<AgentSOHolder>();
            this.agentMischieve = soHolder?.AgentMischieve;
            this.audioCollection = soHolder?.AudioCollections;
            this.damageable = this.AgentObject.GetComponent<Damageable>();
            this.audioSource = this.AgentObject.GetComponent<AudioSource>();

            if (this.navMeshAgent == null || this.agentMischieve == null || this.damageable == null)
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

            this.requestMischieve = ManagersSOHolder.SODifficultySettings.ShouldLaunchActionByDifficulty(this.BaseMischieveProbobality);
            if (requestMischieve) AudioMixerManager.PlayRundomCollectionClip(this.audioSource, this.audioCollection, AudioCollectionName.LoreDropMischieve);
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
        }

        protected override void OnInterrupt() => this.OnDisable();

        public float GetRandomMischieveTime() => ManagersSOHolder.SODifficultySettings.GetMischieveTimeWithModification(Random.Range(this.MaxMischieveTime, this.MaxMischieveTime));
    }
}