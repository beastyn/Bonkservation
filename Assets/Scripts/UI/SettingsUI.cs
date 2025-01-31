namespace UI
{
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SettingsUI : MonoBehaviour
    {
        public void CloseSettings() => UIManager.CloseUI(this.gameObject);
        //public void OpenMainMenu() => ManagersSOHolder.SceneManagerSO.StartMainMenu();

    }
}
