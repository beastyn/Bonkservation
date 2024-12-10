using UI;
namespace Managers.UI
{
    using Player;
    using System;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> MainRoomUIs;
        [SerializeField] List<GameObject> RoomUIs;

        [NonSerialized] public static TopWindowsUI TopWindowsUI;
        List<GameObject> activeRooms;
        static GameObject activeUI;

        void Awake()
        {
            this.SetUpTopUis();
        }

        void OnEnable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent += OnCameraNumRequestEvent;
            ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent += OnScoreSavedEvent;
            PlayerBonkInputController.SettingsToggle += OnSettingsToggle;

            if (this.MainRoomUIs != null) this.activeRooms = this.MainRoomUIs;
        }

        void OnDisable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent -= OnCameraNumRequestEvent;
            ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent -= OnScoreSavedEvent;
            PlayerBonkInputController.SettingsToggle -= OnSettingsToggle;
        }

        void Start()
        {
            this.activeRooms = new();
        }

        void OnCameraNumRequestEvent(int camNum)
        {

            //Main Room view
            if (camNum == 0 && this.activeRooms != this.MainRoomUIs)
            {
                this.activeRooms.Clear();

                this.RoomUIs.ForEach(ui => ui.SetActive(false));
                foreach (var ui in this.MainRoomUIs)
                {
                    ui.SetActive(true);
                    this.activeRooms.Add(ui);
                }
                return;
            }

            this.activeRooms.Clear();
            this.MainRoomUIs.ForEach(ui => ui.SetActive(false));

            //Idol room view
            foreach (var ui in this.RoomUIs)
            {
                var i = 0;
                if (i == 0 || i == camNum)
                {
                    ui.SetActive(true);
                    this.activeRooms.Add(ui);
                }
            }
        }
        public static void ToggleUI(GameObject UI)
        {
            if (activeUI == UI)
            {
                UI.SetActive(false);
                activeUI = null;
                return;
            }
            if (activeUI == null)
                UI.SetActive(true);
            else
            {
                activeUI.SetActive(false);
                UI.SetActive(true);
            }
            activeUI = UI;
        }
        /*
                public static void CloseUI(GameObject UI) { if (UI.activeSelf) UI.SetActive(false); }
                public static void OpenUI(GameObject UI) { if (!UI.activeSelf) UI.SetActive(true); }*/

        public static void OpenScoreBoard() => ToggleUI(TopWindowsUI.ScoreBoard);
        public static void OpenSettings() => ToggleUI(TopWindowsUI.Settings);


        void SetUpTopUis()
        {
            var uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            //Create menu.
            var topUIObj = GameObject.Instantiate(ManagersSOHolder.UIManagerSO.MenuUis);
            TopWindowsUI = topUIObj.GetComponent<TopWindowsUI>();
            var topUICanvas = topUIObj.GetComponent<Canvas>();
            topUICanvas.renderMode = RenderMode.ScreenSpaceCamera;
            topUICanvas.worldCamera = uiCamera;
        }
        void OnSettingsToggle() {if(ManagersSOHolder.GameStateSO.CurrentGameState != GameState.GameOver) OpenSettings(); }
        void OnScoreSavedEvent() => OpenScoreBoard();

        //public void OpenScoreBoard() => this.ScoreBoardUI.SetActive(true);
        //public void CloseScoreBoard() => this.ScoreBoardUI.SetActive(false);

        /*    public void SwitchCanvas(GameObject UI)
            {
                if (activeUI != UI)
                {
                    activeUI.gameObject.SetActive(false);
                    UI.gameObject.SetActive(true);
                    this.activeUI = UI;
                }
            }*/

    }
}
