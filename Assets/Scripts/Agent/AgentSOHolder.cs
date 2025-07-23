using Gameplay;
using UnityEngine;

namespace Agent
{
    public class AgentSOHolder : MonoBehaviour
    {
        [SerializeField] SOEnergy agentEnergy;
        [SerializeField] SOMischieve agentMischieve;
        [SerializeField] SOAudioCollection audioCollections;

        public SOEnergy AgentEnergy => this.agentEnergy;
        public SOMischieve AgentMischieve => this.agentMischieve;
        public SOAudioCollection AudioCollections => this.audioCollections;

        void Awake()
        {
            this.agentEnergy.Reset();
            this.agentMischieve.ResetMischieve();            
        }
    }
}
