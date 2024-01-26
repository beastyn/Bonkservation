
namespace Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "Happiness", menuName = "Bonk/Managers/Happiness")]
    public class EnergySO : ScriptableObject
    {
        public UnityAction<float> EnergyChangeEvent;

        [SerializeField] float startValue = 100;
        [SerializeField] float maxValue = 100;
        [SerializeField] float restoreEnergyValue = 2f;
        float currentValue = 100;
        bool isRegenerating = false;

        public float StartValue => this.startValue;
        public float MaxValue => this.maxValue;
        public float CurrentValue => this.currentValue;
        public float RestoreEnergyValue => this.restoreEnergyValue;
        public bool IsRegenerating => this.isRegenerating;


        public void RemoveEnergy(float damage)
        {
            if (this.currentValue > 0)
                this.currentValue -= damage;

            this.currentValue = Mathf.Clamp(this.currentValue, 0f, this.maxValue);
            this.EnergyChangeEvent?.Invoke(this.currentValue);
        }

        public void SetEnergy(float value)
        {
            this.currentValue = value;
            this.EnergyChangeEvent?.Invoke(value);
        }

        public void SetEnergyRestore(bool isRestoring) => this.isRegenerating = isRestoring;

        public void RestoreEnergy(float tickValue)
        {
            this.currentValue += tickValue;
            this.currentValue = Mathf.Clamp(this.currentValue, 0f, this.maxValue);
            if (this.currentValue >= this.maxValue) this.isRegenerating = false;
        }
    }
}