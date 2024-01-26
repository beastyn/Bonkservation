namespace UI
{
    using AI;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class KillerUI : MonoBehaviour
    {
        [SerializeField] Slider vodKillerProgress;
        [SerializeField] VodKillerSO vodKillerSO;
        // Start is called before the first frame update
        void Start()
        {
            this.vodKillerProgress.value = 0f;
        }

        // Update is called once per frame
        void Update()
        {
           this.vodKillerProgress.value = this.vodKillerSO.CurrFill / this.vodKillerSO.MaxFill;
        }
    }
}