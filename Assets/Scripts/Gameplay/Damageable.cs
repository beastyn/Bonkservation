
namespace Gameplay
{
    using DG.Tweening;
    using UnityEngine;

    public class Damageable : MonoBehaviour
    {
        [SerializeField] EnergySO energySO;
        [SerializeField] Material material;
        [SerializeField] bool checkDanger = false;

        [Header("Hit Shake")]
        [SerializeField] GameObject spriteToShake;
        [SerializeField] Vector3 shakeStrength = new Vector3(2f, 1f, 0f);
        [SerializeField] float shakeDuration = 0.5f;
        [SerializeField] int frequency = 50;

        float dangerLevel = 0f;
        Color damageableMatColor;

        public void SetDangerLevel(float dangerLevel, bool forceReset = false) => this.dangerLevel = forceReset ? 0 : Mathf.Max(new float[] { this.dangerLevel, dangerLevel });

        bool haveProtection = false;

        void Start()
        {
            this.energySO.SetEnergy(this.energySO.StartValue);
            if (material != null && this.checkDanger) this.damageableMatColor = material.GetColor("_Color");
        }

        void Update()
        {
            if (this.damageableMatColor != null && this.checkDanger) this.material.SetColor("_Color", new Color(this.damageableMatColor.r, this.damageableMatColor.g, this.damageableMatColor.b, this.dangerLevel));
        }

        public void InflictDamage(float damage)
        {
            if (!haveProtection)
            {
                this.energySO.RemoveEnergy(damage);
                if (this.spriteToShake != null)
                {
                    if(DOTween.IsTweening(2))
                        DOTween.Restart(2);
                    else
                        DOTween.Shake(() => this.spriteToShake.transform.localPosition, pos => this.spriteToShake.transform.localPosition = pos, this.shakeDuration, this.shakeStrength, this.frequency).SetEase(Ease.InOutBack).SetId(2);
                }
            }
        }

        public void SetProtection(bool value) => this.haveProtection = value;
    }
}
