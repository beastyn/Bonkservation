using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(fileName = "DifficultySettings", menuName = "Bonkservation/Managers/DifficultySettings")]
    public class SODifficultySettings : ScriptableObject
    {
        [SerializeField] AnimationCurve difficultyCurve;
        [SerializeField] int cycleLengthInDays = 20;


        [SerializeField] float minrand = 0f;
        [SerializeField] float maxrand = 1f;
        [SerializeField] int mischieveDifficultyMultiplier = 2;
        [SerializeField] float multiplyer = 100f;

        [SerializeField] float mischieveProbabilityCap = 70;
        [SerializeField] static int currentDifficultyLevel = 1;


        public float GetDifficultyModificator() => Mathf.FloorToInt(TimeManager.CurrentDay/this.cycleLengthInDays) + this.difficultyCurve.Evaluate(TimeManager.CurrentDay);

        public float GetSpeedWithModification(float initialSpeed) => initialSpeed + initialSpeed * this.GetDifficultyModificator();

        public float GetMischieveTimeWithModification(float initialTime) => Mathf.Clamp(initialTime - initialTime*this.GetDifficultyModificator(),1,initialTime);

        public bool ShouldLaunchActionByDifficulty(int basicProbabilityPercent)
        {
            var decisionValue = Random.value;
            var compareValue = basicProbabilityPercent + basicProbabilityPercent * this.GetDifficultyModificator() * this.mischieveDifficultyMultiplier;
            var decision = decisionValue <= Mathf.Clamp(compareValue, 0f, this.mischieveProbabilityCap) / 100f;
            return decision;
        }

        public float GerNormalRandomNum()
        {
            var rand = RandomFromDistribution.RandomRangeNormalDistribution(minrand, maxrand, RandomFromDistribution.ConfidenceLevel_e._999) * multiplyer;
            return rand;
        }
    }
}
