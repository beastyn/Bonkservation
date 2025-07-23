using Managers;
using UnityEngine;

public class BaseWindowUI : MonoBehaviour
{
    public void Close(bool savePrefs = false) => UIManager.CloseUI(this.gameObject, savePrefs);
    public void LoadTitleScreen() => ManagersSOHolder.SOSceneManager.StartMainMenu();
    public void RestartObservation() => ManagersSOHolder.SOSceneManager.StartLevel();
    public void Quit() => ManagersSOHolder.SOSceneManager.Quit();
}
