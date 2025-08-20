using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Gameplay
{
    public class Damageable : MonoBehaviour
    {
        public static System.Action<float> DamageReflectEvent;
        [SerializeField] SOEnergy energySO;
        [SerializeField] float damageReflectValue;
        [SerializeField] bool defaultProtection = true;

        [SerializeField] float FlyAwayForceY = 1000f;
        [SerializeField] float FlyAwayForceXZ = 1000f;
        [SerializeField] float upAngled = 15f;
        [SerializeField] float upNormalized = 0.1f;
        [SerializeField] Rigidbody bodyToThrough;

        public Rigidbody BobbleHead;
        public SoftBodyData SoftBodyData;

        bool haveProtection = false;

        void Start()
        {
            this.energySO.SetEnergy(this.energySO.StartValue);
            this.haveProtection = this.defaultProtection;
        }

        public void InflictDamage(float damage, Transform damageSource = null, NavMeshAgent navMeshAgent = null)
        {
            if (!haveProtection)
            {
                this.energySO.ChangeEnergy(-damage);
                if(damageSource == null) return;

                navMeshAgent.speed = 0f;
                navMeshAgent.enabled = false;
                this.bodyToThrough.isKinematic = false;

                var dirNormal = (this.gameObject.transform.position - damageSource.position).normalized;
                var upward = Vector3.up * this.upNormalized;
                this.bodyToThrough.AddForce(upward * this.FlyAwayForceY, ForceMode.Impulse);
                this.bodyToThrough.AddForce(new Vector3(dirNormal.x, 0f, dirNormal.z) * this.FlyAwayForceXZ, ForceMode.Impulse);
            }
            else DamageReflectEvent?.Invoke(damageReflectValue);
        }

        public void SetProtection(bool value) => this.haveProtection = value;
    }
}
