namespace Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "TimeDamage", menuName = "Bonk/Managers/TimeDamage")]
    public class TimeDamagerSO : ScriptableObject
    {
        [SerializeField] int timeDamage = 5;
        [SerializeField] int timeDamageFrequency = 3;

        public int TimeDamage => this.timeDamage;
        public int TimeDamageFrequency => this.timeDamageFrequency;

    }
}
