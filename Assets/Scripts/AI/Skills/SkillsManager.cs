namespace AI.Skills
{
    using System.Collections;
    using UnityEngine;
    using Gameplay;
    using AI.Skills.Internal;
    using Managers;
    using System.Linq;

    public class SkillsManager : MonoBehaviour
    {
        [SerializeField] Skill[] normalSkills;
        [SerializeField] Damageable player;
        [SerializeField] IdolInfoSO idolInfo;

        public Skill[] NormalSkills => this.normalSkills;

        Skill currentSkill;
        AttackerSpawner[] currentAttackerSpawner;
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

        private void Update()
        {
            if (this.HaveRunningSkill() && (int)ManagersSOHolder.GameStateSO.CurrentGameState != idolInfo.RoomNumber)
            {
                this.currentSkill?.SkillSO.StopSkill();
                return;
            }
            if (!this.HaveRunningSkill() && ManagersSOHolder.DifficultySettingsSO.GerNormalRandomNum() < ManagersSOHolder.DifficultySettingsSO.GetNormalSkillProbability())
            {
                /*    if ((int)ManagersSOHolder.GameStateSO.CurrentGameState != idolInfo.RoomNumber)
                    {
                        this.currentcastingskill?.SkillSO.StopSkill();
                        this.skillWasActivated = false;
                        this.currentcastingskill = null;
                        return Node.Status.Failure;
                    }*/

                var currentcastingskill = this.RequestNormalSkill();
                if (currentcastingskill == null) return;

                this.ActivateSkill(currentcastingskill);
                Debug.LogWarning("Start Skill");
            }
        }

        public Skill RequestNormalSkill() => this.normalSkills[Random.Range(0, normalSkills.Length)];

        public void ActivateSkill(Skill skill)
        {
            if (!this.haveActiveSkill && !this.haveCastingTime)
            {
                this.currentSkill = skill;
                this.currentAttackerSpawner = this.currentSkill.SkillObject.GetComponentsInChildren<AttackerSpawner>();
                foreach(var spawner in this.currentAttackerSpawner)
                    spawner.AttackEndMethod = this.StopAttackerSpawner;
                skill.SkillSO.ActivateSkill();
            }
        }

        public bool HaveActiveNormalSkill() => this.haveActiveSkill;
        public bool HaveCastingTime() => this.haveCastingTime;

        public bool HaveRunningSkill() => this.haveActiveSkill || this.haveCastingTime;

        void OnSkillStartActivateEvent(SkillSO skill)
        {
            this.haveCastingTime = true;
            this.currentSkill.SkillCastEffect.SetActive(true);
        }
        void OnSkillActivatedEvent(SkillSO skill)
        {
            this.haveCastingTime = false;
            this.haveActiveSkill = true;
            this.currentSkill.SkillCastEffect.SetActive(false);
            this.currentSkill.SkillObject.SetActive(true);
            StartCoroutine(this.FinishSkill(skill));
        }
        void OnSkillInterruptedEvent(SkillSO skill)
        {
            if (this.currentAttackerSpawner.Any() && this.haveActiveSkill)
                foreach(var spawner in this.currentAttackerSpawner)
                    spawner?.SwitchSpawner(false);
            this.currentSkill.SkillCastEffect.SetActive(false);
            this.haveCastingTime = false;
        }

        IEnumerator FinishSkill(SkillSO skill)
        {
            yield return new WaitForSeconds(skill.Duration);

            if (this.currentAttackerSpawner.Any() && this.haveActiveSkill)
                foreach (var spawner in this.currentAttackerSpawner)
                    spawner?.SwitchSpawner(false);
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