namespace UI.Time
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Managers;
    using UnityEngine.UI;

    public class TimeUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI timeText;
        [SerializeField] ProgressBarCircle timeBar;

        void OnEnable() => ManagersSOHolder.TimeManagerSO.DayChangeEvent += OnDayChangeEvent;
        void OnDisable() => ManagersSOHolder.TimeManagerSO.DayChangeEvent -= OnDayChangeEvent;

        void Start()
        {
            this.timeText.SetText($"Day 1");
        }
        void Update()
        {
            this.timeBar.BarValue = (ManagersSOHolder.TimeManagerSO.CurrentDayTime / ManagersSOHolder.TimeManagerSO.SecondsPerDay) * 100;
            //this.timeBar.value = this.managers.TimeManagerSO.CurrentDayTime / this.managers.TimeManagerSO.SecondsPerDay;

        }

        void OnDayChangeEvent(int day) => this.timeText.SetText($"Day {day}");
    }
}
