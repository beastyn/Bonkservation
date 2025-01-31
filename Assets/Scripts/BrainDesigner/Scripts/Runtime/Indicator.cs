using UnityEngine;
using BrainDesigner.Scripts.Utils;
using System;
using System.Reflection;

namespace BrainDesigner.Scripts.Runtime
{
    using Utils;

    [Serializable]
    public class Indicator : Named, ICloneable
    {
        public float ChangePerSecond => this.changePerSecond;
        public float CurrentTime => this.currentTime;
        public float LastUpdateTime => this.lastUpdateTime;

        public System.Action ChangeValueAction { get; set; }

        [SerializeField] internal float initialValue;
        [SerializeField] internal bool setMinValue;
        [SerializeField] internal float minValue;
        [SerializeField] internal bool setMaxValue;
        [SerializeField] internal float maxValue;
        [SerializeField] internal bool changedOutside;
        [SerializeField] internal float changePerSecond;
        [SerializeField] internal bool useRealTime;

        internal float currentValue;
        float currentTime;
        float lastUpdateTime;

        internal void Initialize()
        {
            this.currentValue = initialValue;
            this.lastUpdateTime = this.useRealTime ? Time.realtimeSinceStartup : Time.time;
        }
        internal float GetValue() => this.currentValue;
        internal void SetValue(float newValue) => this.currentValue = this.setMinValue && this.currentValue < this.minValue 
                                                                      ? this.minValue : setMaxValue && this.currentValue > this.maxValue 
                                                                                        ? this.maxValue  : newValue;

        internal void Update() 
        {
            if (this.changePerSecond == 0)
                return;
            
            this.currentTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
            
            if (this.changedOutside)
            {
                this.ChangeValueAction?.Invoke();
                this.lastUpdateTime = this.currentTime;
                return;
            }

            this.currentValue += changePerSecond * (currentTime - this.lastUpdateTime);
            this.lastUpdateTime = currentTime;
        }

        public object Clone()
        {
            Type type = this.GetType();
            Indicator newIndicator = (Indicator)Activator.CreateInstance(type);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo[] fields = type.GetFields(flags);

            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(this);
                object copiedValue = Utils.DeepCopy(fieldValue);
                field.SetValue(newIndicator, copiedValue);
            }

            return newIndicator;
        }
        
    }
}
