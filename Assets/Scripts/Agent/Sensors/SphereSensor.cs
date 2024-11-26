using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using System;

namespace Agent
{
    [Serializable]
    public class SphereSensor : Sensor
    {
        public float radius = 5.0f;        // Radius to search for objects

/*        internal SphereSensor(GameObject maneSan, float radius)
        {
            this.maneSan = maneSan;
            this.radius = radius;
        }*/

        protected override void OnUpdate()
        {
            base.OnUpdate();

            List<Transform> foundObjects = new();
            Collider[] colliders = Physics.OverlapSphere(this.AgentObject.transform.position, this.radius);
            foreach (Collider collider in colliders)
                if (collider.gameObject.tag == this.tag)
                    foundObjects.Add(collider.gameObject.transform);
            
            this.SetSensorObject(foundObjects);

            if(foundObjects.Count <=0) this.ClearSensoredObject();
        }
    }
}
