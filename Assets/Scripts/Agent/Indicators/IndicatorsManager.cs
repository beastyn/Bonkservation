using System.Collections.Generic;
using UnityEngine;


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
        const string maneSanSensorName = "Mane Bonk Sensor";

        Indicator fearIndicator;
        Indicator energyIndicator;
        Sensor maneSanSensor;
 
        void Start()
        {
            agentBrain.TryGetIndicatorByName(fearIndicatorName, out this.fearIndicator);
            agentBrain.TryGetIndicatorByName(energyIndicatorName, out this.energyIndicator);
            agentBrain.TryGetSensorByName(maneSanSensorName, out this.maneSanSensor);

            this.agentEnergy ??= this.GetComponent<AgentSOHolder>().AgentEnergy;

            if(this.fearIndicator!=null && this.maneSanSensor != null) this.fearIndicator.ChangeValueAction = this.ChangeFearValue;
            if(this.energyIndicator != null && this.agentEnergy != null) this.agentEnergy.EnergyChangeEvent  += this.OnEnergyChangeEvent;
        }

        void OnDestroy()
        {
            if (this.energyIndicator != null && this.agentEnergy != null) this.agentEnergy.EnergyChangeEvent -= this.OnEnergyChangeEvent;
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
        }

    }
}
