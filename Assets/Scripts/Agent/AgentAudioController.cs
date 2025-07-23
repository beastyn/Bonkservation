using Managers;
using UnityEngine;

namespace Agent
{
    public class AgentAudioController : MonoBehaviour
    {
        [SerializeField] CollisionDetector collisionDerector;
        [SerializeField] AudioSource audioSource;
        SOAudioCollection soAudioCollection;

        void OnEnable()
        {
            this.collisionDerector.HardImpactEvent += OnHardImpactEvent;
        }
        void OnDisable()
        {
            this.collisionDerector.HardImpactEvent -= OnHardImpactEvent;
        }

        void Awake()
        {
            this.collisionDerector ??= this.GetComponent<CollisionDetector>();
            this.audioSource ??= this.GetComponent<AudioSource>();
            this.soAudioCollection= this.GetComponent<AgentSOHolder>().AudioCollections;            
        }
        void OnHardImpactEvent() => AudioMixerManager.PlayRundomCollectionClip(this.audioSource, this.soAudioCollection, AudioCollectionName.GroundImpactSounds);
    }
}
