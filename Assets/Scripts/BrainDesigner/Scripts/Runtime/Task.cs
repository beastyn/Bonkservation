using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BrainDesigner.Scripts.Runtime
{
    using Unity.VisualScripting.YamlDotNet.Core.Tokens;
    using Utils;

    [Serializable]
    public class Task : Named
    {
        internal List<Behaviour> AvailableBehaviours { get => this.avaibleBehaviours; set { this.avaibleBehaviours = value; } }
        internal HashSet<Behaviour> LinkedBehaviours { get => this.linkedBehaviour; set { this.linkedBehaviour = value; } }

        internal string SensorToCheckName { get => this.sensorToCheckName;  set { this.sensorToCheckName = value; } }
        internal string IndicatorToCheckName { get => this.indicatorToCheckName; set { this.indicatorToCheckName = value; } }
        internal Comparator Comparator { get => this.comparator; set { this.comparator = value; } }
        internal float CompareValue { get => this.compareValue; set { this.compareValue = value; } }

        [SerializeField] string sensorToCheckName;
        [SerializeField] string indicatorToCheckName;
        [SerializeField] Comparator comparator;
        [SerializeField] float compareValue;
        [SerializeField] List<Behaviour> avaibleBehaviours;
        [SerializeField] HashSet<Behaviour> linkedBehaviour;

        Sensor sensor;
        Indicator indicator;

        bool lastSensoredFound;
        bool lastConditionMet;
        internal void Initialize(BrainDesigner brainDesigner)
        {
            brainDesigner.TryGetSensorByName(this.sensorToCheckName, out this.sensor);
            brainDesigner.TryGetIndicatorByName(this.indicatorToCheckName, out this.indicator);
        }

        internal bool ConditionMet()
        {
            //If some of condition is non-existend
            if (this.sensor == null)
                this.lastSensoredFound = true;
            if (this.indicator == null)
                this.lastConditionMet= true;

            if (this.sensor == null && this.indicator == null)
                return true;

            //Check for Sensor condition

            if(this.sensor != null) this.lastSensoredFound = this.sensor.TryGetSensoredObject(out List<Transform> foundObjects);

            //Check for indicator cinditions
            var indicatorValue = this.indicator.GetValue();
            switch (comparator)
            {
                case Comparator.GreaterThan:
                    this.lastConditionMet = indicatorValue > this.compareValue;
                    break;
                case Comparator.LessThan:
                    lastConditionMet = indicatorValue < this.compareValue;
                    break;
                case Comparator.GreaterThanOrEqualTo:
                    lastConditionMet = indicatorValue >= this.compareValue;
                    break;
                case Comparator.LessThanOrEqualTo:
                    lastConditionMet = indicatorValue <= this.compareValue;
                    break;
                case Comparator.EqualTo:
                    lastConditionMet = Mathf.RoundToInt(indicatorValue) == Mathf.RoundToInt(this.compareValue);
                    break;
                case Comparator.NotEqualTo:
                    lastConditionMet = Mathf.RoundToInt(indicatorValue) != Mathf.RoundToInt(this.compareValue);
                    break;
                default:
                    lastConditionMet = Mathf.RoundToInt(indicatorValue) == Mathf.RoundToInt(this.compareValue);
                    break;
            }

            return this.lastSensoredFound && this.lastConditionMet;
        }

        internal bool Update() => this.ConditionMet();
    }
}
