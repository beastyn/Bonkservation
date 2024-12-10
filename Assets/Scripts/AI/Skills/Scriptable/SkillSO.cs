namespace AI.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using MEC;
    using Gameplay;

    [CreateAssetMenu(fileName = "Skill", menuName = "Bonk/Skill")]
    public class SkillSO : ScriptableObject
    {
        public UnityAction<SkillSO> SkillStartActivateEvent;
        public UnityAction<SkillSO> SkillActivatedEvent;
        public UnityAction<SkillSO> SkillCastInterruptedEvent;
        public UnityAction<SkillSO> SkillFinishedEvent;

        [SerializeField] float activationTime = 2f;
        [SerializeField] float damage = 10f;
        [SerializeField] Sprite visual;
        [SerializeField] AnimationClip animation;
        [SerializeField] ParticleSystem activationParticles;

        [Header("For repeated projectiles")]
        [SerializeField] float defaulAttackerFrequency = 0.5f;
        [SerializeField] float duration = 5f;

        float currentActivatingTime = 0f;

        bool isActivating = false;

        public float ActivationTime => this.activationTime;
        public float Damage => this.damage;
        public Sprite Visual => this.visual;
        public AnimationClip Animation => this.animation;
        public ParticleSystem ActivationParticles => this.activationParticles;

        public float CurrentActivatingTime => this.currentActivatingTime;
        public bool IsActivating => this.isActivating;

        public float CurrentAttackersFrequency => this.defaulAttackerFrequency * DifficultySettingsSO.GetAttackersSkillDifficultyModificator();
        public float Duration => this.duration;
        public void ActivateSkill()
        {
            this.isActivating = true;
            SkillStartActivateEvent?.Invoke(this);
            Timing.RunCoroutine(FinishCastSequence());
        }
        public void SetCurrentActivatingTime(float time) => this.currentActivatingTime = time;
        public void StopSkill()
        {
            Timing.KillCoroutines();
            this.isActivating = false;
            this.SkillCastInterruptedEvent?.Invoke(this);
        }

        IEnumerator<float> FinishCastSequence()
        {
            yield return Timing.WaitForSeconds(this.activationTime);

            this.isActivating = false;
            this.SkillActivatedEvent?.Invoke(this);
            Timing.RunCoroutine(StartSkillSequence());
        }

        IEnumerator<float> StartSkillSequence()
        {
            yield return Timing.WaitForSeconds(this.duration);
            this.SkillFinishedEvent?.Invoke(this);
        }
    }
}
