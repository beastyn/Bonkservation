using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    using Gameplay;
    public class EnergyUI : MonoBehaviour
    {        
        [SerializeField] Slider energySlider;
        [SerializeField] SOEnergy soEnergy;


        void Awake()
        {
            this.energySlider.value = 1f;
        }

        void OnEnable()
        {
            this.soEnergy.EnergyChangeEvent += OnEnergyChangeEvent;
        }

        void OnEnergyChangeEvent(float enreyValue, bool isReastoring)
        {
            this.energySlider.value = this.soEnergy.CurrentValue / this.soEnergy.MaxValue;
        }
    }
}
