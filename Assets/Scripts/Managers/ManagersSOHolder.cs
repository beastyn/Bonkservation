using UnityEngine;

namespace Managers
{
    public class ManagersSOHolder : MonoBehaviour
    {
        [SerializeField] SOTimeSettings soTimeSettings;
        [SerializeField] SOScoreManager soScoreManager;
        [SerializeField] SOSceneManager soSceneManager;
        [SerializeField] SOAudioManager soAudioManager;
        [SerializeField] SODifficultySettings soDifficultySettings;
        /* [SerializeField] CameraEventsSO cameraEvent;
         [SerializeField] GameStateSO gameStateSO;*/

        /*[SerializeField] TimeDamagerSO timeDamagerSO;
        [SerializeField] DifficultySettingsSO difficultySettingsSO;
        [SerializeField] ScoreManagerSO scoreManagerSO;
        [SerializeField] LightPresetSO lights;
        [SerializeField] AudioManagerSO audioManagerSO;
        [SerializeField] SceneManagerSO sceneManagerSO;*/

        public static SOTimeSettings SOTimeSettings;
        public static SOScoreManager SOScoreManager;
        public static SOSceneManager SOSceneManager;
        public static SOAudioManager SOAudioManager;
        public static SODifficultySettings SODifficultySettings;
        /* 
         public static CameraEventsSO CameraEvent;
          public static GameStateSO GameStateSO;public static TimeDamagerSO TimeDamagerSO;
                public static DifficultySettingsSO DifficultySettingsSO;
                public static ScoreManagerSO ScoreManagerSO;
                public static LightPresetSO Lights;
                public static AudioManagerSO AudioManagerSO;
                public static SceneManagerSO SceneManagerSO;*/

        void Awake()
        {
            SOTimeSettings = this.soTimeSettings;
            SOScoreManager = this.soScoreManager;
            SOSceneManager = this.soSceneManager;
            SOAudioManager = this.soAudioManager;
            SODifficultySettings = this.soDifficultySettings;
            /* CameraEvent = this.cameraEvent;
             GameStateSO = this.gameStateSO;

             TimeDamagerSO = this.timeDamagerSO;
             DifficultySettingsSO = this.difficultySettingsSO;
             ScoreManagerSO = this.scoreManagerSO;
             Lights = this.lights;
             AudioManagerSO = this.audioManagerSO;
             SceneManagerSO = this.sceneManagerSO;*/
        }
    }
}

