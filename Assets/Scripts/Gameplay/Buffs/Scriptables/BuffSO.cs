namespace Gameplay.Buffs
{
    using MEC;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class BuffSO : ScriptableObject
    {
        [SerializeField] float buffDuration = 3f;

        public float BuffDuration => this.buffDuration;
    }
}
