using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    using Agent;
    using Gameplay;

    public class StressManager : MonoBehaviour
    {
        [SerializeField] List<AgentSOHolder> stressDealers;
        [SerializeField] SOEnergy playerStress;

        void Awake() => this.playerStress.Reset();
        void OnEnable()
        {
            foreach(var stressDealer in stressDealers) 
            {
                stressDealer.AgentMischieve.FullMischieveEvent += this.OnFullMischieveEvent;
            }
            Damageable.DamageReflectEvent += OnDamageReflectValue;
        }

        void OnDisable()
        {
            foreach (var stressDealer in stressDealers)
            {
                stressDealer.AgentMischieve.FullMischieveEvent -= this.OnFullMischieveEvent;
            }
            Damageable.DamageReflectEvent -= OnDamageReflectValue;
        }

        void OnFullMischieveEvent(SOMischieve mischieveInfo)
        {
            this.playerStress.ChangeEnergy(mischieveInfo.FilledStressAmount);
        }

        void OnDamageReflectValue(float reflectAmount) 
        {
            this.playerStress.ChangeEnergy(reflectAmount);
        }
    }
}
