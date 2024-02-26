namespace Managers.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> MainRoomUIs;
        [SerializeField] List<GameObject> RoomUIs;
        [SerializeField] GameObject ScoreBoardUI;

        List<GameObject> activeUIs;

        void OnEnable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent += OnCameraNumRequestEvent;
            ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent += OnScoreSavedEvent;

            if(this.MainRoomUIs != null) this.activeUIs = this.MainRoomUIs;
        }

        void OnDisable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent -= OnCameraNumRequestEvent;
            ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent -= OnScoreSavedEvent;
        }

        void Start()
        {
            this.activeUIs = new();
        }

        void OnCameraNumRequestEvent(int camNum)
        {

            //Main Room view
            if (camNum == 0 && this.activeUIs != this.MainRoomUIs)
            {
                this.activeUIs.Clear();

                this.RoomUIs.ForEach(ui => ui.SetActive(false));
                foreach (var ui in this.MainRoomUIs)
                {
                    ui.SetActive(true);
                    this.activeUIs.Add(ui);
                }
                return;
            }

            this.activeUIs.Clear();
            this.MainRoomUIs.ForEach(ui => ui.SetActive(false));

            //Idol room view
            foreach (var ui in this.RoomUIs)
            {
                var i = 0;
                if (i == 0 || i == camNum)
                {
                    ui.SetActive(true);
                    this.activeUIs.Add(ui);
                }
            }
        }

        void OnScoreSavedEvent()
        {
            this.ScoreBoardUI.SetActive(true);
        }

        public static void CloseUI(GameObject UI) => UI.SetActive(false);
        public static void OpenUI(GameObject UI) => UI.SetActive(true);
        public void OpenScoreBoard() => this.ScoreBoardUI?.SetActive(true);

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
