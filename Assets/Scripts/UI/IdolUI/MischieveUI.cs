using Agent;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MischieveUI : MonoBehaviour
    {
        [SerializeField] Slider mischieveSlider;
        [SerializeField] SOMischieve soMischieve;


        void Awake()
        {
            this.mischieveSlider.value = 1f;
        }

        void Update()
        {
            this.mischieveSlider.value = this.soMischieve.CurrFill / this.soMischieve.MaxFill;
        }
    }
}
