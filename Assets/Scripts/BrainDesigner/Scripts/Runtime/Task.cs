using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BrainDesigner.Scripts.Runtime
{
    using Codice.CM.Client.Differences;
    using NUnit.Framework;
    using System.Reflection;
    using Unity.VisualScripting.YamlDotNet.Core.Tokens;
    using Utils;

    [Serializable]
    public class Task : Named, ICloneable
    {
        internal List<Behaviour> LinkedBehaviours => this.linkedBehaviour;

        internal string SensorToCheckName { get => this.sensorToCheckName;  set => this.sensorToCheckName = value; } 
        internal string IndicatorToCheckName { get => this.indicatorToCheckName; set => this.indicatorToCheckName = value; } 
        internal Comparator ComparatorFirst { get => this.comparatorFirst; set => this.comparatorFirst = value; } 
        internal float CompareValueFirst { get => this.compareValueFirst; set => this.compareValueFirst = value; } 
        internal Comparator ComparatorSecond { get => this.comparatorSecond; set => this.comparatorSecond = value; } 
        internal float CompareValueSecond { get => this.compareValueSecond; set => this.compareValueSecond = value; } 
        internal bool NeedSecondCondition { get => this.needSecondCondition; set => this.needSecondCondition = value; } 

        [SerializeField] string sensorToCheckName;
        [SerializeField] string indicatorToCheckName;
        [SerializeField] Comparator comparatorFirst;
        [SerializeField] float compareValueFirst;
        [SerializeField] bool needSecondCondition = false;
        [SerializeField] Comparator comparatorSecond;
        [SerializeField] float compareValueSecond;
        [SerializeReference] List<Behaviour> linkedBehaviour = new();

        Sensor sensor;
        Indicator indicator;

        BrainDesigner brainDesigner;
        bool lastSensoredFound;
        bool lastConditionMet;
        internal void Initialize(BrainDesigner brainDesigner)
        {
            this.brainDesigner = brainDesigner;
            brainDesigner.TryGetSensorByName(this.sensorToCheckName, out this.sensor);
            brainDesigner.TryGetIndicatorByName(this.indicatorToCheckName, out this.indicator);
        }

        internal bool ConditionMet()
        {
            //If some of condition is non-existend
            if (this.sensor == null)
                this.lastSensoredFound = true;
            if (this.indicator == null)
                this.lastConditionMet = true;

            if (this.sensor == null && this.indicator == null)
                return true;

            //Check for Sensor condition

            if (this.sensor != null) this.lastSensoredFound = this.sensor.TryGetSensoredObject(out List<Transform> foundObjects);

            //Check for indicator cinditions
            var indicatorValue = this.indicator.GetValue();
            var firstComparatorMet = true;
            var secondComparatorMet = true;

            firstComparatorMet = this.CheckComparator(this.comparatorFirst, indicatorValue, this.compareValueFirst);
            if (this.needSecondCondition)
                secondComparatorMet = this.CheckComparator(this.comparatorSecond, indicatorValue, this.compareValueSecond);

            this.lastConditionMet = firstComparatorMet && secondComparatorMet;

            return this.lastSensoredFound && this.lastConditionMet;
        }        

        internal bool Update() => this.ConditionMet();

        internal void LinkUniqueBehaviour(Behaviour behaviour)
        {
            if(this.linkedBehaviour.Contains(behaviour)) return;
            this.linkedBehaviour.Add(behaviour);
            behaviour.ActivationTasks.Add(this); 
        }

        internal void UnlinqBehaviorAt(int index, Behaviour behavior)
        {
            this.LinkedBehaviours.RemoveAt(index);
            behavior.ActivationTasks.Remove(this);
        }

        internal void SetBrainDesigner(BrainDesigner brainDesigner) => this.brainDesigner = brainDesigner;

        public object Clone()
        {
            Type type = this.GetType();
            Task newTask = (Task)Activator.CreateInstance(type);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo[] fields = type.GetFields(flags);

            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(this);
                if (fieldValue is IList list && list.Count > 0 && list[0] is Behaviour)
                {
                    foreach (var item in list)
                    {
                        this.brainDesigner.BehaviourSet.TryGetElementByName(((Behaviour)item).Name, out var behaviour);
                        newTask.LinkUniqueBehaviour(behaviour);
                    }
                }
                else
                {
                    object copiedValue = Utils.DeepCopy(fieldValue);
                    field.SetValue(newTask, copiedValue);
                }
            }

            return newTask;
        }

        bool CheckComparator(Comparator comparator, float indicatorValue, float valueToCompare)
        {
            bool comporatorMet;
            switch (comparator)
            {
                case Comparator.GreaterThan:
                    comporatorMet = indicatorValue > valueToCompare;
                    break;
                case Comparator.LessThan:
                    comporatorMet = indicatorValue < valueToCompare;
                    break;
                case Comparator.GreaterThanOrEqualTo:
                    comporatorMet = indicatorValue >= valueToCompare;
                    break;
                case Comparator.LessThanOrEqualTo:
                    comporatorMet = indicatorValue <= valueToCompare;
                    break;
                case Comparator.EqualTo:
                    comporatorMet = Mathf.RoundToInt(indicatorValue) == Mathf.RoundToInt(valueToCompare);
                    break;
                case Comparator.NotEqualTo:
                    comporatorMet = Mathf.RoundToInt(indicatorValue) != Mathf.RoundToInt(valueToCompare);
                    break;
                default:
                    comporatorMet = Mathf.RoundToInt(indicatorValue) == Mathf.RoundToInt(valueToCompare);
                    break;
            }

            return comporatorMet;
        }
    }
}
