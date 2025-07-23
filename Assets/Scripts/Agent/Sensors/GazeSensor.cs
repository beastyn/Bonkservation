using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using System.Collections.Generic;

namespace Agent
{
    public class GazeSensor : Sensor
    {
        public float radius = 5.0f;
        public float lookThreshold = 0.95f; // How closely they need to look (1.0 = perfect)

        [SerializeField] int transformIndex;

        Transform gazeToDetect;

        protected override void RegisterDropdowns()
        {
            AddDropdown("Sight To Check", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
                newIndex => { this.transformIndex = newIndex; });
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this.gazeToDetect = this.SceneRefs.GetRef<Transform>(this.transformIndex);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            List<Transform> foundObjects = new();

            Vector3 toPlayer = (this.AgentObject.transform.position - this.gazeToDetect.position).normalized;
            float dot = Vector3.Dot(this.gazeToDetect.forward, toPlayer);

            if (dot >= lookThreshold)
            {
                foundObjects.Add(this.gazeToDetect);
            }

            this.SetSensorObject(foundObjects);

            if (foundObjects.Count <= 0) this.ClearSensoredObject();
        }
    }
}
