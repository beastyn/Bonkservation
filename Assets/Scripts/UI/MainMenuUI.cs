namespace UI
{
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] GameObject scoreBoard;
        [SerializeField] GameObject settings;

        //public void Play() => ManagersSOHolder.SOScoreManager.StartLevel();
        public void OpenScoreBoard() => UIManager.OpenUI(scoreBoard);
        public void OpenSettings() => UIManager.OpenUI(settings);


    }
}