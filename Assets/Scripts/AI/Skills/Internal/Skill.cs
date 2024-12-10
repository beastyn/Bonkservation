namespace AI.Skills.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Skill
    {
        public SkillSO SkillSO;
        public GameObject SkillObject;
        public GameObject SkillCastEffect;

        public virtual void ActivateSkill() { }

    }
}
