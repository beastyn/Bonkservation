namespace Player.Actions
{
    using Gameplay;
    using Managers;
    using UnityEngine;

    public class BonkAction : ActionBase
    {
        bool canDamage = false;
        Collider2D currentIdol;
        protected override void OnEnable()
        {
            PlayerBonkInputController.MainActionEvent += OnMainActionEvent;
            this.actionSO.ActionEvent += OnActionEvent;
        }
        protected override void OnDisable()
        {
            PlayerBonkInputController.MainActionEvent -= OnMainActionEvent;
            this.actionSO.ActionEvent -= OnActionEvent;
        }

        void OnMainActionEvent()
        {
            this.activated = true;
        }
        void OnActionEvent()
        {
            if (this.canDamage && this.activated)
            {
                this.currentIdol.gameObject.GetComponent<Damageable>()?.InflictDamage(this.actionSO.CurrentDamage);
            }
        }
        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Idol"))
            {
                this.canDamage= true;
                this.currentIdol = collision;
            }
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Idol"))
            {
                this.canDamage = false;
            }
        }
    }
}
