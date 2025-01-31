using UnityEngine;

namespace Gameplay
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] SOEnergy energySO;

        bool haveProtection = false;

        void Start()
        {
            this.energySO.SetEnergy(this.energySO.StartValue);
        }

        public void InflictDamage(float damage)
        {
            if (!haveProtection) this.energySO.ChangeEnergy(-damage);
        }

        public void SetProtection(bool value) => this.haveProtection = value;
    }
}
