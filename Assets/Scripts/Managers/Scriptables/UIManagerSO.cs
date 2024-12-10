namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "UIManager", menuName = "Bonk/Managers/UIManager")]
    public class UIManagerSO : ScriptableObject
    {
        [SerializeField] GameObject menuUIs;

        public GameObject MenuUis => this.menuUIs;
    }
}
