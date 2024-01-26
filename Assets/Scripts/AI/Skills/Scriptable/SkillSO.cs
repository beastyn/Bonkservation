namespace AI.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using MEC;

    [CreateAssetMenu(fileName = "Skill", menuName = "Bonk/Skill")]
    public class SkillSO : ScriptableObject
    {
        public UnityAction<SkillSO> SkillStartActivateEvent;
        public UnityAction<SkillSO> SkillActivatedEvent;
        public UnityAction<SkillSO> SkillInterruptedEvent;

        [SerializeField] float activationTime = 2f;
        [SerializeField] float damage = 10f;
        [SerializeField] Sprite visual;
        [SerializeField] AnimationClip animation;
        [SerializeField] ParticleSystem activationParticles;

        float currentActivatingTime = 0f;
        bool isActivating;
        bool stopCast = false;

        public float ActivationTime => this.activationTime;
        public float Damage => this.damage;
        public Sprite Visual => this.visual;
        public AnimationClip Animation => this.animation;
        public ParticleSystem ActivationParticles => this.activationParticles;

        public float CurrentActivatingTime => this.currentActivatingTime;
        public bool IsActivating => this.isActivating;
        public bool StopCast => this.stopCast;

        public void ActivateSkill()
        {
            this.isActivating = true;
            SkillStartActivateEvent?.Invoke(this);
            Timing.RunCoroutine(_StopActivation());
        }
        public void SetCurrentActivatingTime(float time) => this.currentActivatingTime = time;
        public void SetStopCasting(bool stopCast) => this.stopCast = stopCast;

        IEnumerator<float> _StopActivation()
        {
            yield return Timing.WaitForSeconds(activationTime);
            if (!stopCast)
            {
                this.isActivating = false;
                SkillActivatedEvent?.Invoke(this);
            }
            else
            {
                this.isActivating = false;
                this.stopCast = false;
                SkillInterruptedEvent?.Invoke(this);
            }
        }
    }
}
