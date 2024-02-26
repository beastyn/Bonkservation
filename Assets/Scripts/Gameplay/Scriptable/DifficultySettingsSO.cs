namespace Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Difficulty", menuName = "Bonk/DifficultySettings")]
    public class DifficultySettingsSO : ScriptableObject
    {
        [SerializeField] float minrand = 0f;
        [SerializeField] float maxrand = 1f;
        [SerializeField] float multiplyer = 100f;

        [SerializeField] int baseDodgeProbability = 5;
        [SerializeField] int baseNormalSkillProbablity = 5;
        [SerializeField] static int baseSkillFrequencyModificator = 1;
        [SerializeField] static int currentDifficultyLevel = 1;


        public int GetDodgeProbability() => this.baseDodgeProbability * currentDifficultyLevel;
        public int GetNormalSkillProbability() => this.baseNormalSkillProbablity * currentDifficultyLevel;

        public static float GetAttackersSkillDifficultyModificator() => baseSkillFrequencyModificator / currentDifficultyLevel;

        public float GerNormalRandomNum()
        {
            var rand = RandomFromDistribution.RandomRangeNormalDistribution(minrand, maxrand, RandomFromDistribution.ConfidenceLevel_e._999) * multiplyer;
            return rand;
        }
    }
}
