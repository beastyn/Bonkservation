
namespace Agent
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BonkDetector : MonoBehaviour
    {
        public bool IsBonkerDetected => this.isBonkerDetected;
        public Transform Bonker => this.bonker;

        [SerializeField] Outline outliner;

        bool isBonkerDetected;
        Transform bonker;

        void Start() => this.outliner.enabled = false;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Bonker"))
            {
                this.outliner.enabled = true;
                this.isBonkerDetected= true;
                this.bonker = other.transform;
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
