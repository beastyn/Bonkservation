namespace AI.Skills
{
    using Gameplay;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UIElements;

    public class Attacker : MonoBehaviour
    {
        public UnityAction<Attacker> ReturnMeEvent;

        [SerializeField] SkillSO skillSO;
        //[SerializeField] float timeToReach = 1f;
        [SerializeField] float impactScale = 4f;
        [SerializeField] bool destroyOnScale;
        [SerializeField] bool destroyOutScreen;

        GameObject pawToFollow;
        Damageable damageablePlayer;

        /*Vector3 originalPawPosition;
        Vector3 originalLocalScale;
        float speed;
        float scaleSpeed;*/

        bool canDamage = false;

        void OnEnable()
        {
            this.pawToFollow = PlayerSOHolder.CurrentPaw;
            this.damageablePlayer = this.pawToFollow.transform.GetComponentInParent<Damageable>();
        }

        void OnDisable()
        {
            this.damageablePlayer.SetDangerLevel(0, true);
        }

        void Update()
        {
            var screenPosX = Camera.main.WorldToScreenPoint(this.transform.position).x;
            this.damageablePlayer.SetDangerLevel(1 - (this.impactScale - this.transform.localScale.x)/this.impactScale - (this.canDamage ? 0 : (this.transform.position - this.pawToFollow.transform.position).magnitude / 2f));
            if (this.canDamage)
            {
                this.damageablePlayer.InflictDamage(this.skillSO.Damage);
                this.canDamage = false;
                this.ReturnMeEvent?.Invoke(this);
                return;
            }
            if ((this.destroyOnScale && this.impactScale - this.transform.localScale.x <= 0) || (this.destroyOutScreen && (screenPosX >= (float)Screen.width || screenPosX <= 0)))
                this.ReturnMeEvent?.Invoke(this);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && this.impactScale - this.transform.localScale.x  <= 0.2) this.canDamage = true;
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && this.impactScale - this.transform.localScale.x <= 0.2) this.canDamage = true;
        }
    }
}