namespace AI
{
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class VodKillerController : MonoBehaviour
    {
        [SerializeField] VodKillerSO killerSO;
        [SerializeField] VodKillersSO killersSO;
        [SerializeField] VodKillerSO killerToFill;


        void OnEnable() => ManagersSOHolder.TimeManagerSO.DayChangeEvent += OnDayChangeEvent;
        void OnDisable() => ManagersSOHolder.TimeManagerSO.DayChangeEvent-= OnDayChangeEvent;

        void Start()
        {
            this.killerSO.ResetKiller();
            this.killerSO.SetKillerTransform(this.transform);
        }

        // Update is called once per frame
        void Update()
        {
            if(this.killerSO.IsActive && this.killersSO.CanUse)
                this.killerToFill.UpdateKillerFill((this.killerSO.MaxFill / ManagersSOHolder.TimeManagerSO.SecondsPerDay) * this.killerSO.Speed * Time.deltaTime);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Idol")
                this.killerSO.SetKiller(true);
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Idol")
                this.killerSO.SetKiller(false);
        }
        void OnDayChangeEvent (int day) => this.killerSO.ResetKiller();
    }
}
