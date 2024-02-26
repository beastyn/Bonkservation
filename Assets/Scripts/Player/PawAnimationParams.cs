namespace Player
{
    using UnityEngine;

    public class PawAnimationParams : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] ActionsSO bonkAction;
        [SerializeField] ActionsSO defendAction;

        void Start()
        {
            if (this.animator == null) this.animator = this.gameObject.GetComponent<Animator>(); 
        }

        void OnEnable()
        {
            this.bonkAction.ActionEvent += OnBonkActionEvent;
            this.defendAction.ActionEvent += OnDefendActionEvent;
        }

        void OnDisable()
        {
            this.bonkAction.ActionEvent -= OnBonkActionEvent;
            this.defendAction.ActionEvent -= OnDefendActionEvent;
        }

        void OnBonkActionEvent()
        {
            this.animator.SetTrigger("IsBonking");

        }
        void OnDefendActionEvent()
        {
            this.animator.SetTrigger("IsDefending");
        }
    }
}
