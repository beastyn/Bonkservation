namespace AI
{
    using AI.Skills;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class IdolEffectsController : MonoBehaviour
    {
        [SerializeField] IdolInfoSO idolInfo;
        [SerializeField] TrailRenderer dodgeParticles;
        [SerializeField] SkillsManager skillManager;

        ParticleSystem currentSkillEffect;
        void OnEnable()
        {
            this.idolInfo.DodgeEvent += OnDodgeEvent;

            foreach (var normalSkill in skillManager.NormalSkills)
            {
                //normalSkill.SkillSO.SkillStartActivateEvent += OnSkillStartActivateEvent;
                //normalSkill.SkillSO.SkillActivatedEvent += OnSkillActivatedEvent;
                //normalSkill.SkillSO.SkillCastInterruptedEvent += OnSkillInterruptedEvent;
            }
        }

        void OnDisable()
        {
            this.idolInfo.DodgeEvent -= OnDodgeEvent;

            foreach (var normalSkill in skillManager.NormalSkills)
            {
                //normalSkill.SkillSO.SkillStartActivateEvent -= OnSkillStartActivateEvent;
                //normalSkill.SkillSO.SkillActivatedEvent -= OnSkillActivatedEvent;
                //normalSkill.SkillSO.SkillCastInterruptedEvent -= OnSkillInterruptedEvent;
            }
        }

      /*  void OnSkillStartActivateEvent(SkillSO skill)
        {
            this.currentSkillEffect = GameObject.Instantiate(skill.ActivationParticles, transform.position, Quaternion.identity, this.transform);
        }*/
/*        void OnSkillActivatedEvent(SkillSO skill)
        {
            GameObject.Destroy(this.currentSkillEffect.gameObject);
        }*/
/*        void OnSkillInterruptedEvent(SkillSO skill)
        {
            GameObject.Destroy(this.currentSkillEffect);
        }*/

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
