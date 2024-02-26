namespace Gameplay.Buffs
{
    using Gameplay;
    using Gameplay.Buffs.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.Events;

    public class BuffManager : MonoBehaviour
    {
        public static UnityAction<BuffSO> BuffActivateEvent;
        public static UnityAction<BuffSO> BuffDeactivateEvent;

        [SerializeField] BuffSO fastShieldBuffSO;
        [SerializeField] BuffSO fastRegenBuffSO;
        [SerializeField] BuffSO mediumShieldBuffSO;

        [SerializeField] Damageable objectToProtect;
        [SerializeField] EnergySO objectToRegen;

        ShieldBuff fastShieldBuff;
        RegenBuff fastRegenBuff;
        ShieldBuff mediumShieldBuff;

        static bool haveActiveBuff = false;

        static List <BuffSO> buffsList;
        public static List <BuffSO> BuffsList => buffsList;

        void Awake()
        {
            buffsList = new();
            this.fastShieldBuff = new ShieldBuff(this.objectToProtect, this.fastShieldBuffSO);
            this.fastRegenBuff = new RegenBuff(this.objectToRegen, this.fastRegenBuffSO);
            this.mediumShieldBuff = new ShieldBuff(this.objectToProtect, this.mediumShieldBuffSO);

            this.fastShieldBuffSO.SetBuffAction(this.ActivateFastShield);
            this.fastRegenBuffSO.SetBuffAction(this.ActivateFastRegen);
            this.mediumShieldBuffSO.SetBuffAction(this.ActivateMediumShield);

            buffsList.Add(this.fastShieldBuffSO);
            buffsList.Add(this.fastRegenBuffSO);
            buffsList.Add(this.mediumShieldBuffSO);
        }

        void OnEnable()
        {
            BuffBase.BuffActivationEvent += OnBuffActivationEvent;
            BuffBase.BuffDeactivationEvent += OnBuffDeactivationEvent;
        }
        void OnDisable()
        {
            BuffBase.BuffActivationEvent -= OnBuffActivationEvent;
            BuffBase.BuffDeactivationEvent -= OnBuffDeactivationEvent;
        }

        public void ActivateFastShield() { if (!haveActiveBuff) this.fastShieldBuff.ActivateBuff(this.fastShieldBuffSO.BuffDuration); }
        public void ActivateFastRegen() { if (!haveActiveBuff) this.fastRegenBuff.ActivateBuff(this.fastRegenBuffSO.BuffDuration); }
        public void ActivateMediumShield() { if (!haveActiveBuff) this.mediumShieldBuff.ActivateBuff(this.mediumShieldBuffSO.BuffDuration); }

        public static BuffSO GetRandomBuff()
        {
            var rand = UnityEngine.Random.Range(0, buffsList.Count);
            return buffsList.ElementAt(rand);
        }

        public static bool HaveActiveBuff() => haveActiveBuff;

        void OnBuffActivationEvent(BuffSO buffSO)
        {
            haveActiveBuff = true;
            BuffActivateEvent?.Invoke(buffSO);
        }
        void OnBuffDeactivationEvent(BuffSO buffSO)
        {
            haveActiveBuff = false;
            BuffDeactivateEvent?.Invoke(buffSO);
        }

    }
}