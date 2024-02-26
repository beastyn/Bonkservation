namespace UI
{
    using Managers;
    using Managers.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] GameObject scoreBoard;
        [SerializeField] GameObject settings;

        public void Play() => ManagersSOHolder.SceneManagerSO.StartLevel();
        public void OpenScoreBoard() => UIManager.OpenUI(scoreBoard);
        public void OpenSettings() => UIManager.OpenUI(settings);


    }
}