namespace Gameplay.Buffs
{
    using System.Collections;
    using System.Collections.Generic;
    using MEC;
    using UnityEngine;
    using UnityEngine.Events;


    [CreateAssetMenu(fileName = "BuffGachaInfo", menuName = "Bonk/BuffGachaInfo")]
    public class BuffGachaSO : ScriptableObject
    {
        public UnityAction GachaActivatedEvent;
        public UnityAction GachaStopActivationEvent;

        [SerializeField] float gachaCooldown = 2f;
        bool canActivateGacha = true;

        public bool CanActivateGacha => this.canActivateGacha;

        public void ActivateGacha()
        {
            if (!canActivateGacha) return;

            this.canActivateGacha = false;
            this.GachaActivatedEvent?.Invoke();
            Timing.RunCoroutine(this.RestartGacha());
        }

        IEnumerator<float> RestartGacha()
        {
            yield return Timing.WaitForSeconds(this.gachaCooldown);

            this.canActivateGacha = true;
            this.GachaStopActivationEvent?.Invoke();
        }

        public void PrepareGacha() => this.canActivateGacha = true;
    }
}
