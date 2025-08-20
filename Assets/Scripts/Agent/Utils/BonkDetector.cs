
namespace Agent
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Utils;

    public class BonkDetector : MonoBehaviour
    {
        public bool IsBonkerDetected => this.isBonkerDetected;
        public Transform Bonker => this.bonker;

        [SerializeField] AgentManagersAndData agentManagersAndData;
        [SerializeField] AgentOutliner outliner;

        bool isBonkerDetected;                    
        Transform bonker;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bonker"))
            {
                this.agentManagersAndData.EffectsController.SwitchTargetEffect(true);
                this.isBonkerDetected= true;
                this.outliner.SetEnabled(true);
                this.bonker = other.transform;
            }
        }

        void OnTriggerExit(Collider other) 
        {
            if (other.CompareTag("Bonker"))
            {
                this.agentManagersAndData.EffectsController.SwitchTargetEffect(false);
                this.isBonkerDetected= false;
                this.outliner.SetEnabled(false);
            }
        }
    }
}
