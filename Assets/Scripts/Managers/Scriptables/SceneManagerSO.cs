namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [CreateAssetMenu(fileName = "SceneManager", menuName = "Bonk/Managers/SceneManager")]
    public class SceneManagerSO : ScriptableObject
    {
        public void StartLevel() => SceneManager.LoadScene(1);
        public void StartMainMenu() => SceneManager.LoadScene(0);
    }

}