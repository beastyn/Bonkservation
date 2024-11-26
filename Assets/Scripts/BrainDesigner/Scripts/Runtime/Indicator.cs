using UnityEngine;

using BrainDesigner.Scripts.Utils;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using Unity.Collections.LowLevel.Unsafe;
using Codice.Client.BaseCommands.Changelist;

namespace BrainDesigner.Scripts
{
    [Serializable]
    public class Indicator : Named
    {
        public float ChangePerSecond => this.changePerSecond;
        public float CurrentTime => this.currentTime;
        public float LastUpdateTime => this.lastUpdateTime;

        public Action ChangeValueAction { get; set; }

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
    }
}
