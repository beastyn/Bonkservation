namespace AI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "VodKiller", menuName = "Bonk/VodKiller")]
    public class VodKillerSO : ScriptableObject
    {
        public static UnityAction<VodKillerSO> VodKilledEvent;

        [SerializeField] float maxFill = 100;
        [SerializeField] float speed = 2.0f;
        [SerializeField] int stressAmount = 2;
        [SerializeField] int filledStressAmount = 50;

        Transform killer;
        bool isActive = false;
        float currFill = 0f;

        public bool IsActive => this.isActive;
        public float MaxFill => this.maxFill;
        public float Speed => this.speed;
        public Transform Killer => this.killer;
        public int StressAmount => this.stressAmount;
        public int FilledStressAmount => this.filledStressAmount;

        public float CurrFill => this.currFill;

        public void SetKiller(bool isActive) => this.isActive = isActive;
        public void SetKillerTransform(Transform killerTransform) => this.killer = killerTransform;

        public void UpdateKillerFill(float amountToAdd)
        {
            this.currFill += amountToAdd;
            this.currFill = Mathf.Clamp(this.currFill, 0f, this.maxFill);

            if (this.currFill == this.maxFill)
            {
                VodKilledEvent?.Invoke(this);
                this.currFill = 0f;
            }
        }

        public void ResetKiller()
        {
            this.currFill = 0f;
            this.isActive = false;
        }
    }
}
