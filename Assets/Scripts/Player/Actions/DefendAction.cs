namespace Player.Actions
{
    using Gameplay.Buffs;
    using Player.Actions;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static UnityEngine.InputSystem.PlayerInput;

    public class DefendAction : ActionBase
    {
        [SerializeField] BuffManager buffManager;
        [SerializeField] float protectionTime = 1f;
        bool canDefend = false;
        Collider2D currentIdol;
        protected override void OnEnable()
        {
            PlayerBonkInputController.SecondActionEvent += OnSecondActionEvent;
            this.actionSO.ActionEvent += OnActionEvent;
        }
        protected override void OnDisable()
        {
            PlayerBonkInputController.SecondActionEvent -= OnSecondActionEvent;
            this.actionSO.ActionEvent -= OnActionEvent;
        }

        void OnSecondActionEvent()
        {
            this.activated = true;
        }
        void OnActionEvent()
        {
                this.buffManager.ActivateFastShield();
        }

        /*public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Skill"))
            {
                this.canDefend = true;
            }
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Skill"))
            {
                this.canDefend = false;
            }
        }*/
    }
}
