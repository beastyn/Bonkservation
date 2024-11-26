using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using BrainDesigner.Scripts.Utils;
using NUnit.Framework;
using UnityEngine.VFX;
using UnityEngine.UIElements;

namespace BrainDesigner.Scripts.Runtime
{
    [Serializable]
    public class Sensor: Named
    {
        public string tag;
        public virtual string Description { get; }
        

        protected GameObject AgentObject => this.agentObject;
        
        protected SceneReferences SceneRefs => this.brainDesigner?.sceneReferences;

        [NonSerialized] BrainDesigner brainDesigner;
        [NonSerialized] GameObject agentObject;

        List<Transform> sensoredObjects = new();

        internal void Initialize(BrainDesigner brainDesigner = null, GameObject thisGameObject = null)
        {
            this.brainDesigner= brainDesigner;
            this.agentObject = thisGameObject;
        }

        internal void Update() => this.OnUpdate();

        internal bool TryGetSensoredObject(out List<Transform> foundSensoredObjects)
        {
            foundSensoredObjects = this.sensoredObjects.Count > 0 ? this.sensoredObjects : null;
            return foundSensoredObjects != null;
        }

        protected virtual void OnUpdate() { }
        protected void SetSensorObject(List<Transform> sensoredObjects) => this.sensoredObjects = sensoredObjects;
        protected void ClearSensoredObject() => this.sensoredObjects.Clear();

#if UNITY_EDITOR

        internal void UpdateSensorStateInfo(HelpBox helpBox)
        {
            helpBox.text = TryGetSensoredObject(out var foundObjects) ? $"Detected ojects {foundObjects.Select(item => item.name)}" : "Nothing detected";            
        }
#endif
    }

}
