namespace Gameplay.Buffs
{
    using MEC;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public abstract class BuffBase
    {
        public static UnityAction<BuffBase> BuffActivationEvent;

        protected Damageable damageableObject;
        protected EnergySO energyObject;
        protected BuffSO buffSO;
        bool isActive;

        float currentBuffTime = 0f;

        public void InitWithEnergy(EnergySO energy, BuffSO buffSO)
        {
            this.energyObject = energy;
            this.buffSO = buffSO;
        }
        public void InitWithDamageable(Damageable damageable, BuffSO buffSO)
        {
            this.damageableObject = damageable;
            this.buffSO = buffSO;
        }

        public void ActivateBuff()
        {
            this.isActive = true;
            BuffActivationEvent?.Invoke(this);
            this.ApplyOneTineEffect();
            Timing.RunCoroutine(this.DeactivateBuff());
            Timing.RunCoroutine(this.ProcessEffect());
        }

        IEnumerator<float> DeactivateBuff()
        {
            yield return Timing.WaitForSeconds(this.buffSO.BuffDuration);
            this.RemoveOneTimeEffect();
            this.isActive = false;
        }

        IEnumerator<float> ProcessEffect()
        {
            while (this.isActive)
            {
                this.ApplyContinuousEffect();
                yield return Timing.WaitForOneFrame;
            }
        }

        public virtual void ApplyOneTineEffect() { }
        public virtual void RemoveOneTimeEffect() { }
        public virtual void ApplyContinuousEffect() { }
    }
}
