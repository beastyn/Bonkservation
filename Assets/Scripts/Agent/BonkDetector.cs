
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
            this.outliner.enabled = true;
        }

        void OnTriggerExit(Collider other) 
        {
            this.outliner.enabled = false;
        }
    }
}
