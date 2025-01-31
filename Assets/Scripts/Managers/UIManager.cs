using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject ScoreBoardUI;

        List<GameObject> activeUIs;

        void OnEnable()
        {
            //ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent += OnScoreSavedEvent;

        }

        void OnDisable()
        {
            //ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent -= OnScoreSavedEvent;
        }

        void Start()
        {
            this.activeUIs = new();
        }                

        void OnScoreSavedEvent()
        {
            this.ScoreBoardUI.SetActive(true);
        }

        public static void CloseUI(GameObject UI) => UI.SetActive(false);
        public static void OpenUI(GameObject UI) => UI.SetActive(true);
        public void OpenScoreBoard() => this.ScoreBoardUI?.SetActive(true);
    }
}
