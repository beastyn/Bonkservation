namespace AI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class IdolAnimationParameters : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] IdolAIMovement AIMovementController;

        void Update()
        {
            this.SetWalking(this.AIMovementController.IsWalking);
        }

        void SetWalking(bool isWalking) => animator.SetBool("isWalking", isWalking);
    }
}