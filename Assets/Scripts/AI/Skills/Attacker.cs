namespace AI.Skills
{
    using Gameplay;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class Attacker : MonoBehaviour
    {
        public UnityAction<Attacker> ReturnMeEvent;

        [SerializeField] SkillSO skillSO;
        [SerializeField] float timeToReach = 1f;
        [SerializeField] float impactScale = 4f;

        GameObject pawToFollow;
        Damageable damageablePlayer;

        Vector3 originalPawPosition;
        Vector3 originalLocalScale;
        float speed;
        float scaleSpeed;

        bool canDamage = false;

        void OnEnable()
        {
            this.pawToFollow = PlayerSOHolder.CurrentPaw;
            this.damageablePlayer = this.pawToFollow.transform.GetComponentInParent<Damageable>();
            this.originalPawPosition = this.pawToFollow.transform.position;
            this.originalLocalScale = this.transform.localScale;

            var direction = this.pawToFollow.transform.position - this.transform.position;
            var originalDistance= direction.magnitude;
            this.speed = originalDistance / this.timeToReach;

            this.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        }

        void OnDisable()
        {
            this.transform.localPosition = Vector3.zero;
            this.transform.localScale = this.originalLocalScale;

        }

        void Update()
        {
            this.transform.position += transform.up * Time.deltaTime * this.speed;
            if (this.canDamage)
            {
                this.damageablePlayer.InflictDamage(this.skillSO.Damage);
                this.canDamage = false;
                this.ReturnMeEvent?.Invoke(this);
                return;
            }
            if (this.transform.localScale.x < this.impactScale) this.transform.localScale += (Vector3.right + Vector3.up) * (this.impactScale / this.timeToReach) * Time.deltaTime;
            else if ((this.originalPawPosition - this.transform.position).magnitude <= 0.35) this.ReturnMeEvent?.Invoke(this);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && this.transform.localScale.x - this.impactScale <= 0.5) this.canDamage = true;
        }

/*        public void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) this.canDamage = false;
        }*/


    }
}