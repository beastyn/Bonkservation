namespace Managers.Timer
{
    using UnityEngine;

    public class TimeManager : MonoBehaviour
    {
        void Start() => ManagersSOHolder.TimeManagerSO.ResetDays();

        void Update()
        {
            // Update the time counter based on the elapsed real time
            ManagersSOHolder.TimeManagerSO.AddTime(Time.deltaTime);

            if (ManagersSOHolder.TimeManagerSO.CurrentDayTime / ManagersSOHolder.TimeManagerSO.SecondsPerDay >= (1 - (2f/ ManagersSOHolder.TimeManagerSO.SecondsPerDay)))
                ManagersSOHolder.TimeManagerSO.PrepareToSleep();

            // Check if a day has passed
            if (ManagersSOHolder.TimeManagerSO.CurrentDayTime >= ManagersSOHolder.TimeManagerSO.SecondsPerDay)
            {
                // Increment the day counter
                ManagersSOHolder.TimeManagerSO.SwitchDay();

                // Reset the time counter for the next day
                ManagersSOHolder.TimeManagerSO.ResetCounter();
            }
        }
    }
}
