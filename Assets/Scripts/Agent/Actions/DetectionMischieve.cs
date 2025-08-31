using System.Collections.Generic;
using MEC;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Managers;
using Gameplay;
using static UnityEngine.UI.Image;

namespace Agent
{
    public class DetectionMischieve : Action
    {
        public float MaxDistance = 300.0f;
        public float MinDistance = 5f;
        public float DistanceToStop = 1f;
        public float InitialSpeed = 5f;
        public int BaseProbability = 20;
        public float minTimeToStop = 0.2f;
        public float maxTimeToStop = 0.6f;
        public float maxTimeToMischieve = 5f; //In case agent stopped we need to stop action at somepoint;

        float startTime;
        float currentTime;

        protected override bool NeedSceneRefs => true;

        [SerializeField] int transformIndex;

        AgentManagersAndData agentManagersAndData;

        NavMeshAgent navMeshAgent;
        Damageable damageable;
        BonkDetector bonkDetector;
        AudioSource audioSource;
        SOAudioCollection audioCollection;
        SOMischieve agentMischieve;

        NavMeshSurface navMeshSurface;
        bool requestMischieve;

        Vector3 currDestination = Vector3.zero;
        bool waitStopping = false;
        bool wantUnpause = false;

        protected override void RegisterDropdowns()
        {
            base.RegisterDropdowns();
            AddDropdown("NavMesh Surface Transform", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
              newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            this.agentManagersAndData= this.AgentObject.GetComponent<AgentManagersAndData>();

            this.navMeshAgent = this.agentManagersAndData.NavMeshAgentComponent;
            this.agentMischieve = this.agentManagersAndData?.AgentMischieve;
            this.audioCollection = this.agentManagersAndData?.AudioCollections;
            this.damageable = this.agentManagersAndData?.DamageableComponent;
            this.bonkDetector = this.agentManagersAndData?.BonkDetectorComponent;
            this.audioSource = this.agentManagersAndData.AudioSourceComponent;

            var groundTransform = this.SceneRefs.GetRef<Transform>(this.transformIndex);
            this.navMeshSurface = groundTransform.GetComponent<NavMeshSurface>();

            if (this.agentManagersAndData == null || this.navMeshAgent == null || this.navMeshSurface == null || this.damageable == null || this.bonkDetector == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
            this.requestMischieve = ManagersSOHolder.SODifficultySettings.ShouldLaunchActionByDifficulty(this.BaseProbability);

            if (this.ReferenceMissing ||  !this.navMeshAgent.isOnNavMesh || !this.requestMischieve)
                return;

            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.DistanceToStop;
            this.currDestination = Utils.UtilsTools.GetRandomPointInNavMeshVolume(this.navMeshSurface, this.MaxDistance);
            this.navMeshAgent.destination = this.currDestination;
            this.startTime = Time.realtimeSinceStartup;

            AudioMixerManager.PlayRundomCollectionClip(this.audioSource, this.audioCollection, AudioCollectionName.RumbleMischieves, true);
            this.damageable.SetProtection(false);
            this.agentManagersAndData.EffectsController.SwitchMischieveEffect(true);

        }

        protected override NodeState OnUpdate()
        {
            if (this.ReferenceMissing || !this.navMeshAgent.isOnNavMesh || this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete || !this.requestMischieve)
            {
                return NodeState.Failure;
            }

            this.currentTime = Time.realtimeSinceStartup;

            if (this.navMeshAgent.remainingDistance > this.navMeshAgent.stoppingDistance && (!this.waitStopping || this.currentTime - this.startTime < this.maxTimeToMischieve))
            {
                if (this.bonkDetector.IsBonkerDetected)
                {
                    this.navMeshAgent.updateRotation = false;

                    Vector3 direction = this.bonkDetector.Bonker.position - this.AgentObject.transform.position;
                    direction.y = 0f; // Ignore vertical difference

                    if (direction != Vector3.zero)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        this.AgentObject.transform.rotation = Quaternion.Slerp(this.AgentObject.transform.rotation, lookRotation, 100f * Time.deltaTime);
                    }

                    if (!this.waitStopping)
                    {
                        Timing.RunCoroutine(this.StopMischive());
                        this.waitStopping = true;
                    }
                }
                else if (!this.bonkDetector.IsBonkerDetected && this.wantUnpause)
                {
                    this.navMeshAgent.updateRotation = true;

                    this.waitStopping = false;
                    this.wantUnpause = false;
                    this.navMeshAgent.speed = this.InitialSpeed;
                    this.audioSource.UnPause();
                    this.damageable.SetProtection(false);
                    this.agentManagersAndData.EffectsController.SwitchMischieveEffect(true);
                    this.agentMischieve.UpdateMischieveFill(this.agentMischieve.Speed * Time.deltaTime);
                }
                return NodeState.Running;
            }
            return NodeState.Success;
        }

        protected override void OnDisable()
        {
            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.DistanceToStop;
            this.navMeshAgent.updateRotation = true;

            this.damageable.SetProtection(true);
            this.agentManagersAndData.EffectsController.SwitchMischieveEffect(false);
            this.audioSource.Stop();
            this.waitStopping = false;
        }

        protected override void OnInterrupt() => this.OnDisable();

        IEnumerator<float> StopMischive()
        {
            yield return Timing.WaitForSeconds(Random.Range(this.minTimeToStop, this.maxTimeToStop));

            if (this.waitStopping)
            {
                this.startTime = Time.realtimeSinceStartup;
                this.navMeshAgent.speed = 0f;
                this.audioSource.Pause();
                this.damageable.SetProtection(true);
                this.agentManagersAndData.EffectsController.SwitchMischieveEffect(false);
                this.wantUnpause = true;
            }
        }
    }
}
