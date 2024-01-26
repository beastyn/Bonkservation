namespace Managers
{
    using Managers.Timer;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    public class LightManager : MonoBehaviour
    {
        [SerializeField] Light2D ambientLight;
        [SerializeField] Light2D[] roomsLight;

        int timeOfDay;

        void UpdateLight(float timePercent)
        {
            this.ambientLight.color = ManagersSOHolder.Lights.DirectinalColor.Evaluate(timePercent);

            for (var i=0; i < this.roomsLight.Length; i++)
                this.roomsLight[i].color = ManagersSOHolder.Lights.RoomsColor[i].Evaluate(timePercent);
        }

        void Update()
        {
            if (ManagersSOHolder.Lights == null) return;

            this.UpdateLight(ManagersSOHolder.TimeManagerSO.CurrentDayTime / ManagersSOHolder.TimeManagerSO.SecondsPerDay);

        }

    }
}