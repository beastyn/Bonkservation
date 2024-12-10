namespace UI
{
    using Gameplay;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class EnergyUI : MonoBehaviour
    {
        [SerializeField] Slider energySlider;
        [SerializeField] EnergySO energySO;
        [SerializeField] Transform parent;
        [SerializeField] bool adjustPosition = true;


        float initialY;

        void Awake()
        {
            this.initialY = this.transform.localPosition.y;
            this.energySlider.value = 1f;
        }

        void Update()
        {
            this.energySlider.value = this.energySO.CurrentValue / this.energySO.MaxValue;
        }

        void LateUpdate()
        {
            if (adjustPosition)
            {
                this.transform.rotation = Quaternion.identity;
                var direction = (this.transform.position - this.parent.position).normalized;
                var angle = Vector3.SignedAngle(direction, Vector3.up, Vector3.forward);
                this.transform.localPosition = Quaternion.AngleAxis(angle, Vector3.forward) * this.transform.localPosition;
            }
        }
    }
}
