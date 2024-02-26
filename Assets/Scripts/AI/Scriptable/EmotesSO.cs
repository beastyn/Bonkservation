namespace AI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Emotes", menuName = "Bonk/Emotes")]
    public class EmotesSO : ScriptableObject
    {
        [SerializeField] Sprite bonkEmote;

        public Sprite BonkEmote => this.bonkEmote;

    }
}
