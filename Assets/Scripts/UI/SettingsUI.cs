namespace UI
{
    using Managers;
    using Managers.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SettingsUI : MonoBehaviour
    {
        public void CloseSettings() => UIManager.ToggleUI(this.gameObject);
        public void OpenMainMenu() => ManagersSOHolder.SceneManagerSO.StartMainMenu();

    }
}
