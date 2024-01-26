namespace Player.Actions
{
    using Gameplay;
    using Player.Actions;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static UnityEngine.InputSystem.PlayerInput;

    public class DefendAction : ActionBase
    {
        [SerializeField] Damageable player;
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
            if (this.activated && canDefend)
            {
                this.player.SetProtection(true);
                StartCoroutine(this.RemoveProtection());
            }
        }

        IEnumerator RemoveProtection()
        {
            yield return new WaitForSeconds(this.protectionTime);
            this.player.SetProtection(false);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Skill"))
            {
                Debug.Log("See skill");
                this.canDefend = true;
            }
        }

        public void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Skill"))
            {
                this.canDefend = false;
            }
        }
    }
}
