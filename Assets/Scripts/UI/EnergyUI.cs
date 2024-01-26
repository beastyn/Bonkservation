namespace UI
{
    using Gameplay;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class EnergyUI : MonoBehaviour
    {
        [SerializeField] Slider energySlider;
        [SerializeField] EnergySO happinessSO;
        [SerializeField] Transform parent;

        float initialY;
        /*        void OnEnable() => this.happinessSO.EnergyChangeEvent += OnHappinessChangeEvent;
                void OnDisable() => this.happinessSO.EnergyChangeEvent -= OnHappinessChangeEvent;*/

        void Awake()
        {
            this.initialY = this.transform.localPosition.y;
            this.energySlider.value = 1f;
        }

        void Update()
        {
            this.energySlider.value = this.happinessSO.CurrentValue / this.happinessSO.MaxValue;
        }

        void LateUpdate()
        {
            this.transform.rotation = Quaternion.identity;
            var direction = (this.transform.position - this.parent.position).normalized;
            var angle =Vector3.SignedAngle(direction, Vector3.up, Vector3.forward);
            this.transform.localPosition = Quaternion.AngleAxis(angle, Vector3.forward) * this.transform.localPosition;

        }
    }
}
