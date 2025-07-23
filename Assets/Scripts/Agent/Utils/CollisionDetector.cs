using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

namespace Agent
{
    public class CollisionDetector : MonoBehaviour
    {
        public Action HardImpactEvent; 
        internal bool MischieveCollided => this.mischieveCollided;
        internal GameObject MischieveCollidedObject => this.mischieveCollidedObject;
        internal bool IsHardGroundImpact => isHardGorundImpact;

        [SerializeField] float minVelocityCountHard;

        bool mischieveCollided = false;

        GameObject mischieveCollidedObject = null;

        bool isHardGorundImpact = false;


        void OnTriggerEnter(Collider other)
        {
            if (other == null) return;
            if (other.CompareTag("Mischieve"))
            {
                this.mischieveCollided = true;
                this.mischieveCollidedObject = other.gameObject;
            }

        }

        void OnTriggerExit(Collider other)
        {
            if (other == null) return;
            if (other.CompareTag("Mischieve"))
            {
                this.mischieveCollided = false;
            }

            if (this.mischieveCollidedObject == other.gameObject)
                this.mischieveCollidedObject = null;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision == null) return;

            if (collision.collider.CompareTag("ColliderSound"))
            {
                float impactSpeed = collision.relativeVelocity.magnitude;
                this.isHardGorundImpact = impactSpeed > this.minVelocityCountHard;

                if (this.IsHardGroundImpact) HardImpactEvent?.Invoke();
            }
        }
        void OnCollisionExit(Collision collision)
        {

            if (collision == null) return;

            if (collision.collider.CompareTag("ColliderSound"))
            {
                this.isHardGorundImpact = false;
            }

        }
    }
}
