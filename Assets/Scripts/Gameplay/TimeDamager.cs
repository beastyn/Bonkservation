namespace Gameplay
{
    using Managers;
    using Player;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TimeDamager : MonoBehaviour
    {
        [SerializeField] Damageable player;

        int damageTimes = 1;
        // Update is called once per frame
        void Update()
        {
            if (ManagersSOHolder.TimeManagerSO.CurrentDayTime > this.damageTimes * ManagersSOHolder.TimeManagerSO.SecondsPerDay / ManagersSOHolder.TimeDamagerSO.TimeDamageFrequency)
            {
                player.InflictDamage(ManagersSOHolder.TimeDamagerSO.TimeDamage);
                this.damageTimes++;
                this.damageTimes = Mathf.Clamp(this.damageTimes, 1, ManagersSOHolder.TimeDamagerSO.TimeDamageFrequency);
            }

        }
    }
}
