using System.Collections.Generic;
using UnityEngine;


namespace Agent
{
    using BrainDesigner.Scripts;
    using BrainDesigner.Scripts.Runtime;
    using NUnit.Framework;

    public class IndicatorsManager : MonoBehaviour
    {
        [SerializeField] BrainDesigner agentBrain;

        const string fearIndicatorName = "Fear";
        const string maneSanSensorName = "Mane Bonk Sensor";
        Indicator fearIndicator;
        Sensor maneSanSensor;

        void Start()
        {
            agentBrain.TryGetIndicatorByName(fearIndicatorName, out this.fearIndicator);
            agentBrain.TryGetSensorByName(maneSanSensorName, out this.maneSanSensor);

            if(this.fearIndicator!=null && this.maneSanSensor != null) this.fearIndicator.ChangeValueAction = this.ChangeFearValue;
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

            Debug.Log($"Current fear level {this.agentBrain.GetIndicatorValue(this.fearIndicator)}");
        }
    }
}
