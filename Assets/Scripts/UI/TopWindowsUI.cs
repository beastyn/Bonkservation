namespace UI
{
    using Managers;
    using Managers.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TopWindowsUI : MonoBehaviour
    {
        [SerializeField] GameObject scoreBoard;
        [SerializeField] GameObject settings;

        public GameObject ScoreBoard => this.scoreBoard;
        public GameObject Settings => this.settings;

    }
}