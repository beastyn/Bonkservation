namespace AI.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UI;
    using Gameplay;

    public class SkillsManager : MonoBehaviour
    {
        [SerializeField] SkillSO[] normalSkills;
        [SerializeField] Transform normalSkillObject;
        [SerializeField] Animation normalSkillAnim;
        [SerializeField] SpriteRenderer normalSkillSprite;
        [SerializeField] Damageable player;

        public SkillSO[] NormalSkills => this.normalSkills;

        bool haveActiveSkill = false;

        void OnEnable()
        {
            foreach (var skillSO in this.normalSkills)
            {
                skillSO.SkillStartActivateEvent += this.OnSkillStartActivateEvent;
                skillSO.SkillActivatedEvent += this.OnSkillActivatedEvent;
                skillSO.SkillInterruptedEvent += this.OnSkillInterruptedEvent;
            }
        }

        void OnDisable()
        {
            foreach (var skillSO in this.normalSkills)
            {
                skillSO.SkillStartActivateEvent -= this.OnSkillStartActivateEvent;
                skillSO.SkillActivatedEvent -= this.OnSkillActivatedEvent;
                skillSO.SkillInterruptedEvent -= this.OnSkillInterruptedEvent;
            }
        }

        public SkillSO RequestNormalSkill() => this.normalSkills[Random.Range(0, normalSkills.Length)];

        public void ActivateSkill(SkillSO skill) { if (!this.haveActiveSkill) skill.ActivateSkill(); }

        /*void Update()
        {
            foreach(var normalSkill in normalSkills)
            {
                if(normalSkill.IsActivating)
                {
                    if (normalSkill.CurrentActivatingTime < normalSkill.ActivationTime)
                    {
                        normalSkill.SetCurrentActivatingTime(normalSkill.CurrentActivatingTime + Time.deltaTime);
                        //Play animation
                    }
                    else
                    {
                        normalSkill.SetSkillActivation(false);
                        normalSkill.SetCurrentActivatingTime(0f);
                        if (!normalSkill.StopCast)
                        {
                            this.normalSkillParent.gameObject.SetActive(true);
                            StartCoroutine(this.FinishSkill(normalSkill));
                        }
                        normalSkill.SetStopCasting(false);
                    }
                }
            }
        }*/

        public bool HaveActiveNormalSkill() => this.haveActiveSkill;

        void OnSkillStartActivateEvent(SkillSO skill) => this.haveActiveSkill = true;
        void OnSkillActivatedEvent(SkillSO skill)
        {
            this.haveActiveSkill = false;

            this.normalSkillSprite.sprite = skill.Visual;
            this.normalSkillAnim.clip = skill.Animation;
            this.normalSkillObject.gameObject.SetActive(true);

            StartCoroutine(this.FinishSkill(skill));
        }
        void OnSkillInterruptedEvent(SkillSO skill) => this.haveActiveSkill = false;

        IEnumerator FinishSkill(SkillSO skill)
        {
            yield return new WaitForSeconds(skill.Animation.averageDuration);

            this.player.InflictDamage(skill.Damage);
            this.normalSkillObject.gameObject.SetActive(false);
        }
    }
}