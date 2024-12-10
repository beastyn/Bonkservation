namespace UI
{
    using Managers;
    using Managers.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MenuWindowsUI : MonoBehaviour
    {
        public void Play() => ManagersSOHolder.SceneManagerSO.StartLevel();
        public void OpenScoreBoard() => UIManager.OpenScoreBoard();
        public void OpenSettings() => UIManager.OpenSettings();


    }
}