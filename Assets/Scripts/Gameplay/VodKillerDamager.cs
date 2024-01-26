
namespace Gameplay
{
    using AI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Player;

    public class VodKillerDamager : MonoBehaviour
    {
        [SerializeField] Damageable player;
        void OnEnable() => VodKillerSO.VodKilledEvent += OnVodKilledEvent;
        void OnDisable() => VodKillerSO.VodKilledEvent = OnVodKilledEvent;

        void OnVodKilledEvent(VodKillerSO vodKiller)
        {
            player.InflictDamage(vodKiller.FilledStressAmount);
        }
    }
}
