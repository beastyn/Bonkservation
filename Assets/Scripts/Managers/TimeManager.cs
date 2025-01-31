using System;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    using Gameplay;

    public class TimeManager : MonoBehaviour
    {
        public static int CurrentDay => currentDay;

        [SerializeField] TextMeshProUGUI dayText;
        [SerializeField] TextMeshProUGUI timeText;

        [SerializeField] Light sun;
        [SerializeField] Light moon;
        [SerializeField] Light cloudedSun;
        [SerializeField] AnimationCurve lightIntensityCurve;
        [SerializeField] float maxSunIntensity = 1;
        [SerializeField] float maxMoonIntensity = 0.5f;

        [SerializeField] Color dayAmbientLight;
        [SerializeField] Color nightAmbientLight;
        [SerializeField] Color daySunLight;
        [SerializeField] Color nightSunLight;
        [SerializeField] Volume volume;
        [SerializeField] Material skyboxMaterial;

        [SerializeField] RectTransform dial;
        float initialDialRotation;

        ColorAdjustments colorAdjustments;

        public event Action OnSunrise
        {
            add => service.OnSunrise += value;
            remove => service.OnSunrise -= value;
        }

        public event Action OnSunset
        {
            add => service.OnSunset += value;
            remove => service.OnSunset -= value;
        }

        public event Action OnHourChange
        {
            add => service.OnHourChange += value;
            remove => service.OnHourChange -= value;
        }

        static bool gameIsPaused = false;
        
        static int currentDay = 1;        
        TimeService service;

        
        public static void SetPause(bool pause)
        {
            if (!gameIsPaused && pause)
            {
                gameIsPaused = true;
                Time.timeScale = 0;
            }
            if (gameIsPaused && !pause)
            {
                gameIsPaused = false;
                Time.timeScale = 1;
            }
        }

        void Start()
        {
            service = new TimeService(ManagersSOHolder.SOTimeSettings);
            volume.profile.TryGet(out colorAdjustments);
            OnHourChange += () => OnHourChangeAction();
/*            OnSunrise += () => Debug.Log("Sunrise");
            OnSunset += () => Debug.Log("Sunset");
            OnHourChange += () => Debug.Log("Hour change");*/

            //initialDialRotation = dial?.rotation.eulerAngles.z;
        }

        void Update()
        {
            UpdateTimeOfDay();
            RotateSun();
            UpdateLightSettings();
            UpdateSkyBlend();
/*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                timeSettings.timeMultiplier *= 2;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                timeSettings.timeMultiplier /= 2;
            }*/
        }

        void UpdateSkyBlend()
        {
            float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);
            float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));
            skyboxMaterial.SetFloat("_Blend", blend);
        }

        void UpdateLightSettings()
        {
            float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
            float lightIntensity = lightIntensityCurve.Evaluate(dotProduct);

            //sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensity);
            //moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightIntensity);

            this.cloudedSun.intensity = Mathf.Lerp(maxMoonIntensity, maxSunIntensity, lightIntensity);
            this.cloudedSun.color = Color.Lerp(this.nightSunLight, this.daySunLight, lightIntensity);

            RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensity);
        }

        void RotateSun()
        {
            float fakeSunRotation = service.CalculateSunAngle();
            float cloudedSunRotation = service.CalculateCookieAngle();
            this.sun.transform.rotation = Quaternion.AngleAxis(fakeSunRotation, Vector3.right);
            cloudedSun.transform.rotation = Quaternion.AngleAxis(cloudedSunRotation, Vector3.right);
            //dial?.rotation = Quaternion.Euler(0, 0, rotation + initialDialRotation);
        }

        void UpdateTimeOfDay()
        {
            service.UpdateTime(Time.deltaTime);
            if (timeText != null)
                timeText.text = service.CurrentTime.ToString("HH:mm");

            
        }

        void OnHourChangeAction()
        {
            if (this.service.CurrentTime.TimeOfDay.Hours == ManagersSOHolder.SOTimeSettings.startHour)
            {
                currentDay++;

                if (this.dayText != null)
                    dayText.text = "Day " + currentDay;
            }
           
        }
    }
}