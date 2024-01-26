namespace Managers
{
    using Gameplay;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerSOHolder : MonoBehaviour
    {
        [SerializeField] EnergySO playerSanity;

        public static EnergySO PlayerSanity;

        void Awake()
        {
            PlayerSanity = this.playerSanity;
        }
    }
}
