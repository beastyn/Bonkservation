
namespace Agent
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BonkDetector : MonoBehaviour
    {
        public bool IsBonkerDetected => this.isBonkerDetected;

        [SerializeField] Outline outliner;

        bool isBonkerDetected;

        void Start() => this.outliner.enabled = false;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bonker"))
            {
                this.outliner.enabled = true;
                this.isBonkerDetected= true;
            }
        }

        void OnTriggerExit(Collider other) 
        {
            if (other.CompareTag("Bonker"))
            {
                this.outliner.enabled = false;
                this.isBonkerDetected= false;
            }
        }
    }
}
