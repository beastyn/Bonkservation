using UnityEngine;

namespace Gameplay
{
    public class Damageable : MonoBehaviour
    {
        public static System.Action<float> DamageReflectEvent;
        [SerializeField] SOEnergy energySO;
        [SerializeField] float damageReflectValue;
        [SerializeField] bool defaultProtection = true;
        public Rigidbody BobbleHead;

        bool haveProtection = false;

        void Start()
        {
            this.energySO.SetEnergy(this.energySO.StartValue);
            this.haveProtection = this.defaultProtection;
        }

        public void InflictDamage(float damage)
        {
            if (!haveProtection) this.energySO.ChangeEnergy(-damage);
            else DamageReflectEvent?.Invoke(damageReflectValue);
        }

        public void SetProtection(bool value) => this.haveProtection = value;
    }
}
