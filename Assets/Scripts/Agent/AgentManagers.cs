using UnityEngine;
using UnityEngine.AI;

using Gameplay;

namespace Agent
{
    public class AgentManagersAndData : MonoBehaviour
    {
        public SOEnergy AgentEnergy => this.agentEnergy;
        public SOMischieve AgentMischieve => this.agentMischieve;
        public SOAudioCollection AudioCollections => this.audioCollections;
        public IndicatorsManager IndicatorsManager => this.indicatorsManager;
        public EffectsController EffectsController => this.effectsController;
        public NavMeshAgent NavMeshAgentComponent => this.navMeshAgentComponent;
        public Rigidbody MainRigidbodyComponent => this.mainRigidbodyComponent;
        public Damageable DamageableComponent => this.damageableComponent;
        public BonkDetector BonkDetectorComponent => this.bonkDetectorComponent;
        public CollisionDetector CollisionDetectorComponent => this.collisionDetectorComponent;
        public AudioSource AudioSourceComponent => this.audioSourceComponent;
        public Animator AnimatorComponent => this.animatorComponent; 

        [Header("Agent SO")]
        [SerializeField] SOEnergy agentEnergy;
        [SerializeField] SOMischieve agentMischieve;
        [SerializeField] SOAudioCollection audioCollections;

        [Header("Agent Managers")]
        [SerializeField] IndicatorsManager indicatorsManager;
        [SerializeField] EffectsController effectsController;

        [Header("Agent Components")]
        [SerializeField] NavMeshAgent navMeshAgentComponent;
        [SerializeField] Rigidbody mainRigidbodyComponent;
        [SerializeField] Damageable damageableComponent;
        [SerializeField] BonkDetector bonkDetectorComponent;
        [SerializeField] CollisionDetector collisionDetectorComponent;
        [SerializeField] AudioSource audioSourceComponent;
        [SerializeField] Animator animatorComponent;

        void Awake()
        {
            this.agentEnergy.Reset();
            this.agentMischieve.ResetMischieve();            
        }
    }
}
