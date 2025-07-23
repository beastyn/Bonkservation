using UnityEngine;
using UnityEngine.Events;

namespace Agent
{
    [CreateAssetMenu(fileName = "Mischieve Settings", menuName = "Bonkservation/Idols/Mischieves Settings")]
    public class SOMischieve : ScriptableObject
    {
        public UnityAction<SOMischieve> FullMischieveEvent;

        [SerializeField] float maxFill = 100;
        [SerializeField] float speed = 2.0f;
        [SerializeField] int stressAmount = 2;
        [SerializeField] int filledStressAmount = 50;

        float currFill = 0f;

        public float MaxFill => this.maxFill;
        public float Speed => this.speed;
        public int StressAmount => this.stressAmount;
        public int FilledStressAmount => this.filledStressAmount;

        public float CurrFill => this.currFill;

        public void UpdateMischieveFill(float amountToAdd)
        {
            this.currFill += amountToAdd;
            this.currFill = Mathf.Clamp(this.currFill, 0f, this.maxFill);

            if (this.currFill == this.maxFill)
            {
                FullMischieveEvent?.Invoke(this);
                this.ResetMischieve();
            }
        }

        public void ResetMischieve() => this.currFill = 0;
    }
}