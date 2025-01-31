using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agent
{
    public class CollisionDetector : MonoBehaviour
    {
        internal bool MischieveCollided => this.mischieveCollided;
        internal GameObject MischieveCollidedObject => this.mischieveCollidedObject;

        bool mischieveCollided = false;

        GameObject mischieveCollidedObject = null;


        void OnTriggerEnter(Collider other)
        {
            if(other == null) return;
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

            if(this.mischieveCollidedObject == other.gameObject)
                this.mischieveCollidedObject = null;
        }

    }
}
