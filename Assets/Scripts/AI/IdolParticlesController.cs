namespace AI
{
    using AI.Skills;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class IdolParticlesController : MonoBehaviour
    {
        [SerializeField] IdolInfoSO idolInfo;
        [SerializeField] TrailRenderer dodgeParticles;
        [SerializeField] SkillsManager skillManager;

        ParticleSystem currentSkillEffect;
        void OnEnable()
        {
            this.idolInfo.DodgeEvent += OnDodgeEvent;

            foreach (var normalSkills in skillManager.NormalSkills)
            {
                normalSkills.SkillStartActivateEvent += OnSkillStartActivateEvent;
                normalSkills.SkillActivatedEvent += OnSkillActivatedEvent;
                normalSkills.SkillInterruptedEvent += OnSkillInterruptedEvent;
            }
        }

        void OnDisable()
        {
            this.idolInfo.DodgeEvent -= OnDodgeEvent;

            foreach (var normalSkills in skillManager.NormalSkills)
            {
                normalSkills.SkillStartActivateEvent -= OnSkillStartActivateEvent;
                normalSkills.SkillActivatedEvent -= OnSkillActivatedEvent;
                normalSkills.SkillInterruptedEvent -= OnSkillInterruptedEvent;
            }
        }

        void OnSkillStartActivateEvent(SkillSO skill)
        {
            this.currentSkillEffect = GameObject.Instantiate(skill.ActivationParticles, transform.position, Quaternion.identity, this.transform);
        }
        void OnSkillActivatedEvent(SkillSO skill)
        {
            GameObject.Destroy(this.currentSkillEffect.gameObject);
        }
        void OnSkillInterruptedEvent(SkillSO skill)
        {
            GameObject.Destroy(this.currentSkillEffect);
        }

        void OnDodgeEvent()
        {
            this.dodgeParticles.gameObject.SetActive(true);
            StartCoroutine(this.SwitchDodgeOff());
        }
        IEnumerator SwitchDodgeOff()
        {
            yield return new WaitForSeconds(this.dodgeParticles.time);

            this.dodgeParticles.gameObject.SetActive(false);
        }
    }
}
