namespace Player
{
    using Gameplay.Buffs;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerEffectsController : MonoBehaviour
    {
        ParticleSystem currentEffect;
        void OnEnable()
        {
            BuffManager.BuffActivateEvent += OnBuffActivateEvent;
            BuffManager.BuffDeactivateEvent += OnBuffDeactivateEvent;
        }

        void OnDisable()
        {
            BuffManager.BuffActivateEvent -= OnBuffActivateEvent;
            BuffManager.BuffDeactivateEvent -= OnBuffDeactivateEvent;
        }

        void OnBuffActivateEvent(BuffSO buffSO)
        {
            if(buffSO.Effect != null) this.currentEffect = GameObject.Instantiate(buffSO.Effect, this.transform);
        }

        void OnBuffDeactivateEvent(BuffSO buffSO)
        {
            GameObject.Destroy(this.currentEffect);
        }
    }
}
