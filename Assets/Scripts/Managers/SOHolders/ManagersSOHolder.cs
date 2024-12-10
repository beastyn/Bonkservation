namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using Gameplay;
    using Managers.Buttons;
    using Managers.Cameras;
    using Managers.Timer;
    using Player;
    using UnityEngine;

    public class ManagersSOHolder : MonoBehaviour
    {
        [SerializeField] CameraEventsSO cameraEvent;
        [SerializeField] GameStateSO gameStateSO;
        [SerializeField] TimeManagerSO timeManagerSO;
        [SerializeField] TimeDamagerSO timeDamagerSO;
        [SerializeField] DifficultySettingsSO difficultySettingsSO;
        [SerializeField] ScoreManagerSO scoreManagerSO;
        [SerializeField] LightPresetSO lights;
        [SerializeField] AudioManagerSO audioManagerSO;
        [SerializeField] SceneManagerSO sceneManagerSO;
        [SerializeField] UIManagerSO uiManagerSO;

        public static CameraEventsSO CameraEvent;
        public static GameStateSO GameStateSO;
        public static TimeManagerSO TimeManagerSO;
        public static TimeDamagerSO TimeDamagerSO;
        public static DifficultySettingsSO DifficultySettingsSO;
        public static ScoreManagerSO ScoreManagerSO;
        public static LightPresetSO Lights;
        public static AudioManagerSO AudioManagerSO;
        public static SceneManagerSO SceneManagerSO;
        public static UIManagerSO UIManagerSO;



        void Awake()
        {
            CameraEvent = this.cameraEvent;
            GameStateSO = this.gameStateSO;
            TimeManagerSO = this.timeManagerSO;
            TimeDamagerSO = this.timeDamagerSO;
            DifficultySettingsSO= this.difficultySettingsSO;
            ScoreManagerSO = this.scoreManagerSO;
            Lights = this.lights;
            AudioManagerSO = this.audioManagerSO;
            SceneManagerSO = this.sceneManagerSO;
            UIManagerSO = this.uiManagerSO;
        }
    }
}