using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject scoreBoardUI;
        [SerializeField] GameObject pauseUI;

        List<GameObject> activeUIs;

        void OnEnable()
        {
            //ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent += OnScoreSavedEvent;
            PlayerInputReciever.PauseUIToggle += OnPauseUIToggle;
            

        }

        void OnDisable()
        {
            //ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent -= OnScoreSavedEvent;
            PlayerInputReciever.PauseUIToggle -= OnPauseUIToggle;
        }

        void Start()
        {
            this.activeUIs = new();
        }                

        void OnScoreSavedEvent()
        {
            this.scoreBoardUI.SetActive(true);
        }

        public static void CloseUI(GameObject UI, bool savePrefs = false)
        {
            UI.SetActive(false);
            TimeManager.SetPause(false);
            if (savePrefs) PlayerPrefs.Save();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        public static void OpenUI(GameObject UI)
        {
            UI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        public void OpenScoreBoard() => this.scoreBoardUI?.SetActive(true);

        void OnPauseUIToggle()
        {
            TimeManager.SetPause(true);
            OpenUI(this.pauseUI);
        }
    }
}
