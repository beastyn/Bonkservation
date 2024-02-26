namespace AI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "VodKillers", menuName = "Bonk/VodKillers")]
    public class VodKillersSO : ScriptableObject
    {
        public UnityAction<VodKillerSO> VodKillerChanged;

        [SerializeField] VodKillerSO[] vodKillers;

        bool canUse = false;
        VodKillerSO currentKiller;

        public VodKillerSO CurrentKiller => this.currentKiller;
        public VodKillerSO[] VodKillers => this.vodKillers;
        public bool CanUse => this.canUse;

        public void SetVodKiller(VodKillerSO vodKiller)
        {
            currentKiller = vodKiller;
            this.VodKillerChanged?.Invoke(this.currentKiller);
        }

        public void SetKillerUsage(bool canUse)
        {
            this.canUse = canUse;
        }

    }
}