using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Utils;
using UnityEngine.UIElements;
using System.Reflection;

namespace BrainDesigner.Scripts.Runtime
{
    using Utils;

    [Serializable]
    public class Sensor : NamedWithDropdown, ICloneable
    {
        public string tag;
        public virtual string Description { get; }


        protected GameObject AgentObject => this.agentObject;

        protected override SceneReferences SceneRefs => this.brainDesigner?.sceneReferences;

        [NonSerialized] BrainDesigner brainDesigner;
        [NonSerialized] GameObject agentObject;

        List<Transform> sensoredObjects = new();
        bool enabled = false;

        internal void Initialize(BrainDesigner brainDesigner = null, GameObject thisGameObject = null)
        {
            this.brainDesigner = brainDesigner;
            this.agentObject = thisGameObject;
        }

        internal void Update()
        {
            if(!enabled)
            {
                this.OnEnable();
                this.enabled= true;

            }
            this.OnUpdate();
        }

        internal bool TryGetSensoredObject(out List<Transform> foundSensoredObjects)
        {
            foundSensoredObjects = this.sensoredObjects.Count > 0 ? this.sensoredObjects : null;
            return foundSensoredObjects != null;
        }

        protected virtual void OnEnable() { }
        protected virtual void OnUpdate() { }
        protected void SetSensorObject(List<Transform> sensoredObjects) => this.sensoredObjects = sensoredObjects;
        protected void ClearSensoredObject() => this.sensoredObjects.Clear();

#if UNITY_EDITOR

        internal void UpdateSensorStateInfo(HelpBox helpBox)
        {
            helpBox.text = TryGetSensoredObject(out var foundObjects) ? $"Detected ojects {foundObjects.Select(item => item.name)}" : "Nothing detected";
        }
#endif

        public object Clone()
        {
            Type type = this.GetType();
            Sensor newSensor = (Sensor)Activator.CreateInstance(type);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo[] fields = type.GetFields(flags);

            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(this);
                object copiedValue = Utils.DeepCopy(fieldValue);
                field.SetValue(newSensor, copiedValue);
            }

            return newSensor;
        }
    }
}
