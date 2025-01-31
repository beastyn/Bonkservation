using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    [CreateAssetMenu(fileName = "Energy Settings", menuName = "Bonkservation/Energy Settings", order = 1)]
    public class SOEnergy : ScriptableObject
    {
        /// <summary>Notify if energy was changed with amount data and tue if restores, false if removed.</summary>
        public UnityAction<float, bool> EnergyChangeEvent;
        public UnityAction FullEnergyEvent;

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

        public void ChangeEnergy(float deltaValue)
        {
            this.currentValue += deltaValue;

            this.currentValue = Mathf.Clamp(this.currentValue, 0f, this.maxValue);
            this.EnergyChangeEvent?.Invoke(this.currentValue, deltaValue>0);

            this.NotifyEnergyFull();
        }

        public void SetEnergy(float value)
        {
            this.currentValue = value;
            this.EnergyChangeEvent?.Invoke(value, true);

            this.NotifyEnergyFull();
        }

        public void SetEnergyRestore(bool isRestoring) => this.isRegenerating = isRestoring;

        public void FullRestoreEnergy(float tickValue)
        {
            this.currentValue += tickValue;
            this.currentValue = Mathf.Clamp(this.currentValue, 0f, this.maxValue);
            if (this.currentValue >= this.maxValue) this.isRegenerating = false;
            this.EnergyChangeEvent?.Invoke(this.currentValue, true);
            this.NotifyEnergyFull();
        }

        public void RestoreEnergyTick(float tickValue)
        {
            this.currentValue += tickValue;
            this.currentValue = Mathf.Clamp(this.currentValue, 0f, this.maxValue);
            this.EnergyChangeEvent?.Invoke(this.currentValue, true);
            this.NotifyEnergyFull();
        }
        public void Reset() => this.currentValue = this.startValue;

        void NotifyEnergyFull()
        {
            if (this.currentValue == this.maxValue)
                this.FullEnergyEvent?.Invoke();
        }
    }
}
