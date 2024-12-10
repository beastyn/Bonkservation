namespace Managers
{
    using Gameplay;
    using Player;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;

    public class PlayerSOHolder : MonoBehaviour
    {
        [SerializeField] EnergySO playerSanity;

        static GameObject currentPaw;

        public static EnergySO PlayerSanity;
        public static GameObject CurrentPaw => currentPaw;

        void Awake()
        {
            PlayerSanity = this.playerSanity;
        }

        public static void SetPaw(GameObject paw) => currentPaw = paw;
    }
}
