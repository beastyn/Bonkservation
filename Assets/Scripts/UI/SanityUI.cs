
namespace UI
{
    using Gameplay;
    using Managers;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SanityUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI sanityText;
        [SerializeField] Slider sanityBar;

        void OnEnable() => PlayerSOHolder.PlayerSanity.EnergyChangeEvent += OnHappinessChangeEvent;
        void OnDisable() => PlayerSOHolder.PlayerSanity.EnergyChangeEvent -= OnHappinessChangeEvent;

        void Start() => this.sanityBar.value = 1f;

        void OnHappinessChangeEvent(float currentHap, bool isReastoring)
        {
            this.sanityBar.value = (float)PlayerSOHolder.PlayerSanity.CurrentValue / (float)PlayerSOHolder.PlayerSanity.MaxValue;
        }
    }
}
