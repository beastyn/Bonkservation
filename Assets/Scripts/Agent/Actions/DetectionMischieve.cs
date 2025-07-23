using System.Collections.Generic;
using MEC;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Managers;
using Gameplay;

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

        protected override bool NeedSceneRefs => true;

        [SerializeField] int transformIndex;

        NavMeshAgent navMeshAgent;
        NavMeshSurface navMeshSurface;
        bool requestMischieve;
        Damageable damageable;
        BonkDetector bonkDetector;
        AgentSOHolder soHolder;
        AudioSource audioSource;
        SOAudioCollection audioCollection;
        SOMischieve agentMischieve;

        IndicatorsManager indicatorsManager;

        Vector3 currDestination = Vector3.zero;
        bool waitStopping = false;

        protected override void RegisterDropdowns()
        {
            base.RegisterDropdowns();
            AddDropdown("NavMesh Surface Transform", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
              newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();

            var groundTransform = this.SceneRefs.GetRef<Transform>(this.transformIndex);
            this.navMeshSurface = groundTransform.GetComponent<NavMeshSurface>();
            this.soHolder = this.AgentObject.GetComponent<AgentSOHolder>();
            this.agentMischieve = soHolder?.AgentMischieve;
            this.audioCollection = soHolder?.AudioCollections;
            this.damageable = this.AgentObject.GetComponent<Damageable>();
            this.bonkDetector = this.AgentObject.GetComponent<BonkDetector>();
            this.audioSource = this.AgentObject.GetComponent<AudioSource>();

            this.indicatorsManager = this.AgentObject.GetComponent<IndicatorsManager>();

            if (this.navMeshAgent == null || this.navMeshSurface == null || this.soHolder == null || this.damageable == null || this.indicatorsManager == null)
                this.ReferenceMissing = true;
        }

        protected override void OnEnable()
        {
            this.requestMischieve = ManagersSOHolder.SODifficultySettings.ShouldLaunchActionByDifficulty(this.BaseProbability);

            if (this.ReferenceMissing || !this.requestMischieve)
                return;

            this.navMeshAgent.speed = this.InitialSpeed;
            this.navMeshAgent.stoppingDistance = this.DistanceToStop;
            this.currDestination = this.GetRandomPointInNavMeshVolume();
            this.navMeshAgent.destination = this.currDestination;

            AudioMixerManager.PlayRundomCollectionClip(this.audioSource, this.audioCollection, AudioCollectionName.LoreRumbleMischieve, true);
            this.damageable.SetProtection(false);

        }

        protected override NodeState OnUpdate()
        {
            if (this.ReferenceMissing || this.navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete || !this.requestMischieve)
            {
                return NodeState.Failure;
            }
            if (this.navMeshAgent.remainingDistance > this.navMeshAgent.stoppingDistance)
            {
                if (this.bonkDetector.IsBonkerDetected && !this.waitStopping)//this.indicatorsManager.IsManeSanLooking())
                {                    
                    Timing.RunCoroutine(this.StopMischive());
                    this.waitStopping = true;
                }
                else if (!this.bonkDetector.IsBonkerDetected)
                {
                    this.waitStopping = false;
                    this.navMeshAgent.speed = this.InitialSpeed;
                    this.audioSource.UnPause();
                    this.damageable.SetProtection(false);
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
            this.damageable.SetProtection(true);
            this.audioSource.Stop();
            this.waitStopping = false;
        }

        protected override void OnInterrupt() => this.OnDisable();

        IEnumerator<float> StopMischive()
        {
            yield return Timing.WaitForSeconds(Random.Range(this.minTimeToStop, this.maxTimeToStop));

            if (this.waitStopping)
            {
                this.navMeshAgent.speed = 0f;
                this.audioSource.Pause();
                this.damageable.SetProtection(true);
            }
        }

        Vector3 GetRandomPointInNavMeshVolume()
        {
            var tries = 0;
            

            // Get the bounds of the NavMesh surface
            Bounds navMeshBounds = this.navMeshSurface.navMeshData.sourceBounds;

            Vector3 randomPoint;
            NavMeshHit hit;

            // Try to generate a point inside the volume bounds
            do
            {
                randomPoint = new Vector3(
                    Random.Range(navMeshBounds.min.x, navMeshBounds.max.x), // Random x
                    navMeshBounds.center.y,                                // Fixed Y level
                    Random.Range(navMeshBounds.min.z, navMeshBounds.max.z)  // Random z
                );

                // Check if the random point is on the NavMesh within maxDistance
            } while (!NavMesh.SamplePosition(randomPoint, out hit, this.MaxDistance, NavMesh.AllAreas) && (hit.position - this.AgentObject.transform.position).magnitude <= this.MinDistance);

            return hit.position; // Return the valid point inside the NavMesh volume
        }
    }
}
