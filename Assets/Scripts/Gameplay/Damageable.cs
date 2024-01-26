
namespace Gameplay
{
    using UnityEngine;

    public class Damageable : MonoBehaviour
    {
        [SerializeField] EnergySO energySO;

        bool haveProtection = false;

        void Start()
        {
            this.energySO.SetEnergy(this.energySO.StartValue);
        }

        public void InflictDamage(float damage)
        {
            if (!haveProtection) this.energySO.RemoveEnergy(damage);
        }

        public void SetProtection(bool value) => this.haveProtection = value;
    }
}
