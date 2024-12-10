namespace Gameplay.Buffs.Internal
{
    using MEC;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public abstract class BuffBase
    {
        public static UnityAction<BuffSO> BuffActivationEvent;
        public static UnityAction<BuffSO> BuffDeactivationEvent;

        protected Damageable damageableObject;
        protected EnergySO energyObject;
        protected BuffSO buffSO;
        bool isActive;

        public BuffBase() { }

        public BuffBase(EnergySO energy, BuffSO buffSO)
        {
            this.energyObject = energy;
            this.buffSO = buffSO;
        }
        public bool ActivateBuff(float duration)
        {
            this.isActive = true;
            this.ApplyOneTimeEffect();
            Timing.RunCoroutine(this.DeactivateBuff(duration));
            Timing.RunCoroutine(this.ProcessEffect());
            BuffActivationEvent?.Invoke(this.buffSO);

            return true;
        }

        IEnumerator<float> DeactivateBuff(float duration)
        {
            yield return Timing.WaitForSeconds(duration);
            this.RemoveOneTimeEffect();
            this.isActive = false;
            BuffDeactivationEvent?.Invoke(this.buffSO);
        }

        IEnumerator<float> ProcessEffect()
        {
            while (this.isActive)
            {
                this.ApplyContinuousEffect();
                yield return Timing.WaitForOneFrame;
            }
        }

        public virtual void ApplyOneTimeEffect() { }
        public virtual void RemoveOneTimeEffect() { }
        public virtual void ApplyContinuousEffect() { }
    }
}
