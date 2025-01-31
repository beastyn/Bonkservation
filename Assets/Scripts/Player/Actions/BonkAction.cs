using UnityEngine;

namespace Player.Actions
{
    using Gameplay;
    public class BonkAction : ActionBase
    {
        bool canDamage = false;
        Collider currentIdol;

        protected override void OnEnable()
        {
            PlayerInputReciever.MainActionEvent += OnMainActionEvent;
        }
        protected override void OnDisable()
        {
            PlayerInputReciever.MainActionEvent -= OnMainActionEvent;
        }

        void OnMainActionEvent() => this.activated = true;

        protected override void OnDoAction ()
        {
           if(this.canDamage)
                this.currentIdol.gameObject.GetComponent<Damageable>()?.InflictDamage(this.CurrentDamage);

        }
        public void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Idol"))
            {
                this.canDamage = true;
                this.currentIdol = collision;
            }
        }

        public void OnTriggerExit(Collider collision)
        {
            if (collision.CompareTag("Idol"))
                this.canDamage = false;
        }
    }
}

