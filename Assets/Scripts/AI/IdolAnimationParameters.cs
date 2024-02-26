namespace AI
{
    using Gameplay;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

    public class IdolAnimationParameters : MonoBehaviour
    {
        [SerializeField] EnergySO idolEnergy;
        [SerializeField] Animator animator;
        [SerializeField] IdolAIMovement AIMovementController;

        void Start() { if(this.animator == null) this.animator = this.gameObject.GetComponent<Animator>(); }

        void OnEnable() => this.idolEnergy.EnergyChangeEvent += OnEnergyChangeEvent;
        void OnDisable() => this.idolEnergy.EnergyChangeEvent -= OnEnergyChangeEvent;

        void Update()
        {
            this.SetWalking(this.AIMovementController.IsWalking);
        }

        void SetWalking(bool isWalking) => this.animator.SetBool("isWalking", isWalking);

        void OnEnergyChangeEvent(float energy, bool isRestoring) { if (!isRestoring) this.animator.SetBool("IsBonked", true); }
    }
}