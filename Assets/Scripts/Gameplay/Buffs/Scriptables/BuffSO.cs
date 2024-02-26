namespace Gameplay.Buffs
{
    using MEC;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "Buff", menuName = "Bonk/Buff")]
    public class BuffSO : ScriptableObject
    {
        [SerializeField] string buffName = "Buff name";
        [SerializeField] string description = "Buff description";
        [SerializeField] float buffDuration = 1f;
        [SerializeField] float specificValue = 0f;
        [SerializeField] ParticleSystem effect;
        [SerializeField] Sprite buffItemVisual;

        Action buffAction;
        public string BuffName => this.buffName;
        public string Description => this.description;
        public float BuffDuration => this.buffDuration;
        public float SpecificValue => this.specificValue;
        public ParticleSystem Effect => this.effect;
        public Sprite BuffItemVisual => this.buffItemVisual;
        public Action BuffAction => this.buffAction;

        public void SetBuffAction(Action action) => this.buffAction = action;

    }
}
