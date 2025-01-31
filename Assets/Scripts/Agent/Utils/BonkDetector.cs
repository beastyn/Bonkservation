
namespace Idol
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BonkDetector : MonoBehaviour
    {
        [SerializeField] Outline outliner;

        void Start() => this.outliner.enabled = false;

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Bonker"))
                this.outliner.enabled = true;
        }

        void OnTriggerExit(Collider other) 
        {
            if (other.CompareTag("Bonker")) 
                this.outliner.enabled = false;
        }
    }
}
