namespace AI.Skills
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UI;
    using Gameplay;
    using AI.Skills.Internal;
    using UnityEditor.Experimental.GraphView;
    using Unity.VisualScripting;
    using Managers;

    public class SkillsManager : MonoBehaviour
    {
        [SerializeField] Skill[] normalSkills;
        [SerializeField] Damageable player;
        [SerializeField] IdolInfoSO idolInfo;

        public Skill[] NormalSkills => this.normalSkills;

        Skill currentSkill;
        AttackerSpawner currentAttackerSpawner;
        bool haveActiveSkill = false;
        bool haveCastingTime = false;

        void OnEnable()
        {
            foreach (var skill in this.normalSkills)
            {
                skill.SkillSO.SkillStartActivateEvent += this.OnSkillStartActivateEvent;
                skill.SkillSO.SkillActivatedEvent += this.OnSkillActivatedEvent;
                skill.SkillSO.SkillCastInterruptedEvent += this.OnSkillInterruptedEvent;
            }
            ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;
        }

        void OnDisable()
        {
            foreach (var skill in this.normalSkills)
            {
                skill.SkillSO.SkillStartActivateEvent -= this.OnSkillStartActivateEvent;
                skill.SkillSO.SkillActivatedEvent -= this.OnSkillActivatedEvent;
                skill.SkillSO.SkillCastInterruptedEvent -= this.OnSkillInterruptedEvent;
            }
            ManagersSOHolder.GameStateSO.GameStateChangedEvent -= OnGameStateChangeEvent;
        }

        public Skill RequestNormalSkill() => this.normalSkills[Random.Range(0, normalSkills.Length)];

        public void ActivateSkill(Skill skill)
        {
            if (!this.haveActiveSkill && !this.haveCastingTime)
            {
                skill.SkillSO.ActivateSkill();
                this.currentSkill = skill;
                this.currentAttackerSpawner = this.currentSkill.SkillObject.GetComponent<AttackerSpawner>();
                if(this.currentAttackerSpawner!=null) this.currentAttackerSpawner.AttackEndMethod = this.StopAttackerSpawner;
            }
        }

        public bool HaveActiveNormalSkill() => this.haveActiveSkill;
        public bool HaveCastingTime() => this.haveCastingTime;

        void OnSkillStartActivateEvent(SkillSO skill)
        {
            this.haveCastingTime = true;
        }
        void OnSkillActivatedEvent(SkillSO skill)
        {
            this.haveActiveSkill = false;
            this.haveActiveSkill = true;

            this.currentSkill.SkillObject.SetActive(true);
            StartCoroutine(this.FinishSkill(skill));
        }
        void OnSkillInterruptedEvent(SkillSO skill)
        {
            if (this.currentAttackerSpawner != null && this.haveActiveSkill)
                this.currentAttackerSpawner?.SwitchSpawner(false);
        }

        IEnumerator FinishSkill(SkillSO skill)
        {
            yield return new WaitForSeconds(skill.Duration);

            if (this.currentAttackerSpawner != null && this.haveActiveSkill)
                this.currentAttackerSpawner?.SwitchSpawner(false);
        }
        void StopAttackerSpawner()
        {
            this.currentSkill.SkillObject.gameObject.SetActive(false);
            this.haveCastingTime = false;
            this.haveActiveSkill = false;
        }

        void OnGameStateChangeEvent(GameState gameState)
        {
            if ((int)ManagersSOHolder.GameStateSO.CurrentGameState != this.idolInfo.RoomNumber && (this.haveActiveSkill || this.haveCastingTime))
            {
                this.currentSkill.SkillSO.StopSkill();
            }
        }
    }
}