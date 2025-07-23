using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;


namespace Agent
{
    using BrainDesigner.Scripts;
    using BrainDesigner.Scripts.Runtime;
    using Gameplay;
    using NUnit.Framework;
    using System.Linq;

    public class IndicatorsManager : MonoBehaviour
    {
        [SerializeField] BrainDesigner agentBrain;
        [SerializeField] SOEnergy agentEnergy;

        const string fearIndicatorName = "Fear";
        const string energyIndicatorName = "Energy";
        const string nightIndicatorName = "Night";
        const string bonkedIndicatorName = "Bonked";

        const string maneSanSensorName = "Mane Bonk Sensor";
        const string maneGazeSensorName = "Mane Gaze Sensor";

        Indicator fearIndicator;
        Indicator energyIndicator;
        Indicator nightIndicator;
        Indicator bonkedIndicator;
        //Indicator morningIndicator;

        Sensor maneSanSensor;
        Sensor maneGazeSensor;

        void Start()
        {
            agentBrain.TryGetIndicatorByName(fearIndicatorName, out this.fearIndicator);
            agentBrain.TryGetIndicatorByName(energyIndicatorName, out this.energyIndicator);
            agentBrain.TryGetIndicatorByName(nightIndicatorName, out this.nightIndicator);
            agentBrain.TryGetIndicatorByName(bonkedIndicatorName, out this.bonkedIndicator);

            agentBrain.TryGetSensorByName(maneSanSensorName, out this.maneSanSensor);
            agentBrain.TryGetSensorByName(maneGazeSensorName, out this.maneGazeSensor);

            this.agentEnergy ??= this.GetComponent<AgentSOHolder>().AgentEnergy;

            if (this.fearIndicator != null && this.maneSanSensor != null) this.fearIndicator.ChangeValueAction = this.ChangeFearValue;
            if ((this.energyIndicator != null || this.bonkedIndicator != null) && this.agentEnergy != null) this.agentEnergy.EnergyChangeEvent  += this.OnEnergyChangeEvent;
            if (this.nightIndicator != null)
            {
                TimeManager.SunriseEvent += OnSunriseEvent;
                TimeManager.SunsetEvent += OnSunsetEvent;
            }
        }

        void OnDestroy()
        {
            if ((this.energyIndicator != null || this.bonkedIndicator != null) && this.agentEnergy != null) this.agentEnergy.EnergyChangeEvent -= this.OnEnergyChangeEvent;
            if (this.nightIndicator != null)
            {
                TimeManager.SunriseEvent -= OnSunriseEvent;
                TimeManager.SunsetEvent -= OnSunsetEvent;
            }
        }

        public Transform GetManeSan()
        {
            Transform maneSan = null;
            if (maneSanSensor != null)
            {
                this.agentBrain.TryGetSensoredObjects(this.maneSanSensor, out var foundObhects);
                maneSan = foundObhects?.FirstOrDefault();
            }
            return maneSan;
        }

        public bool IsManeSanLooking()
        {
            Transform maneSan = null;
            if (this.maneGazeSensor != null)
            {
                this.agentBrain.TryGetSensoredObjects(this.maneGazeSensor, out var foundObhects);
                return foundObhects?.FirstOrDefault() != null;
            }
            return false;
        }

        void ChangeFearValue()
        {
            var newValueIncrease = this.agentBrain.GetIndicatorValue(this.fearIndicator) + this.fearIndicator.ChangePerSecond * (this.fearIndicator.CurrentTime - this.fearIndicator.LastUpdateTime);
            var newValueDecrease = this.agentBrain.GetIndicatorValue(this.fearIndicator) - this.fearIndicator.ChangePerSecond * (this.fearIndicator.CurrentTime - this.fearIndicator.LastUpdateTime);

            List<Transform> sensoredObjects;
            if (this.agentBrain.TryGetSensoredObjects(this.maneSanSensor, out sensoredObjects))
                this.agentBrain.SetIndicatorValue(this.fearIndicator, newValueIncrease);
            else
                this.agentBrain.SetIndicatorValue(this.fearIndicator, newValueDecrease);
        }
        void OnEnergyChangeEvent(float value, bool isRestoring)
        {
            this.agentBrain.SetIndicatorValue(this.energyIndicator, this.agentEnergy.CurrentValue);
            if (!isRestoring)
            {
                this.agentBrain.SetIndicatorValue(this.bonkedIndicator, this.bonkedIndicator.MaxValue);
                StartCoroutine(NotBonkedAnymore());
            }
        }

        void OnSunriseEvent()
        {
            this.agentBrain.SetIndicatorValue(this.nightIndicator, this.nightIndicator.MinValue);            
        }

        void OnSunsetEvent()
        {
            this.agentBrain.SetIndicatorValue(this.nightIndicator, this.nightIndicator.MaxValue);
        }

        IEnumerator NotBonkedAnymore()
        {
            yield return new WaitForSeconds(0.5f);
            this.agentBrain.SetIndicatorValue(this.bonkedIndicator, this.bonkedIndicator.MinValue);
        }
    }
}
