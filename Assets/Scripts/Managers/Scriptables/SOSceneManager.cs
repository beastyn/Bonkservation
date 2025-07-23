namespace Managers
{
    using UnityEngine;
    using UnityEngine.SceneManagement;


    [CreateAssetMenu(fileName = "SceneManager", menuName = "Bonkservation/Managers/SceneManager")]
    public class SOSceneManager : ScriptableObject
    {
        public void StartLevel() => SceneManager.LoadScene(1);
        public void StartMainMenu() => SceneManager.LoadScene(0);
        public void Quit() => Application.Quit();
    }
}
