namespace Managers.Timer
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "Time", menuName = "Bonk/Managers/Time")]
    public class TimeManagerSO : ScriptableObject
    {

        public UnityAction<int> DayChangeEvent;
        public UnityAction PrepareToSleepEvent;

        [SerializeField] float secondsPerDay = 60.0f; // Adjust this value to control the speed of time. Use multiplication from 60 (to count in kinda minutes).


        float timeCounter = 0.0f;
        int currentDay = 1;

        public float SecondsPerDay => this.secondsPerDay;
        public int CurrentDay => this.currentDay;
        public float CurrentDayTime => this.timeCounter;

        public void AddTime(float time) => this.timeCounter += time;

        public void ResetDays()
        {
            this.timeCounter = 0f;
            this.currentDay = 1;
        }

        public void ResetCounter() => this.timeCounter = 0f;

        public void SwitchDay()
        {
            this.currentDay++;
            this.DayChangeEvent?.Invoke(this.currentDay);
        }

        public void PrepareToSleep() => this.PrepareToSleepEvent?.Invoke();
    }
}
