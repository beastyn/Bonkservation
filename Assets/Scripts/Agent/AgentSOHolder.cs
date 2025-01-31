using Gameplay;
using UnityEngine;

namespace Agent
{
    public class AgentSOHolder : MonoBehaviour
    {
        [SerializeField] SOEnergy agentEnergy;
        [SerializeField] SOMischieve agentMischieve;

        public SOEnergy AgentEnergy => this.agentEnergy;
        public SOMischieve AgentMischieve => this.agentMischieve;

        void Awake()
        {
            this.agentEnergy.Reset();
            this.agentMischieve.ResetMischieve();
            
        }
    }
}
