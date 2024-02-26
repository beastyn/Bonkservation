namespace AI.Tree.Idols
{
    using Gameplay;
    using Managers;
    using Player;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using AI.Skills;
    using AI.Skills.Internal;

    public class PawAvoidHelper : MonoBehaviour
    {
        [SerializeField] IdolInfoSO idolInfo;
        [SerializeField] EnergySO idolEnergy;
        [SerializeField] SkillsManager skillManager;

        [SerializeField] NavMeshAgent agent;
        [SerializeField] Transform paw;
        [SerializeField] PlayerBonkMovementBehaviour playerBonkMovementBehaviour;

        [SerializeField] float minDistance = 0.3f;
        [SerializeField] float minLookAhead = 0.5f;
        [SerializeField] float runRadius = 3f;
        [SerializeField] float ignoreDistance = 2f;
        [SerializeField] float searchAngle = 90f;

        [SerializeField] float dodgeMinLookAhead = 0.3f;
        [SerializeField] float dodgeCheckRadius = 0.3f;
        [SerializeField] float dodgeRadius = 0.5f;

        Vector3 randomDestination = Vector3.zero;
        bool destinationExists = false;
        bool skillWasActivated = false;
        Skill currentcastingskill = null;

        AIState aiState = AIState.Idle;
        public Node.Status CheckPaw()
        {
            if (idolInfo.RoomNumber == (int)ManagersSOHolder.GameStateSO.CurrentGameState)
                return Node.Status.Success;

            this.randomDestination = Vector3.zero;
            this.destinationExists = false;
            return Node.Status.Failure;
        }

        public Node.Status CheckNeedDodge()
        {
            var rand = Random.Range(1, 100);
            var pawIsFar = this.PawIsFar(this.transform.position - this.paw.position, this.dodgeCheckRadius);

            if (rand < ManagersSOHolder.DifficultySettingsSO.GetDodgeProbability() && !pawIsFar)
            {
                Debug.LogWarning("Want to dodge");
                return Node.Status.Success;
            }

            return Node.Status.Failure;
        }

        public Node.Status DodgePaw()
        {
            var direction = this.transform.position - this.paw.position;
            this.agent.speed = this.idolInfo.DodgeSpeed;
            this.agent.acceleration = this.idolInfo.DodgeAcceleration;
            this.idolInfo.SetRotSpeed(this.idolInfo.DodgeRotSpeed);

            if (this.aiState == AIState.Idle)
                this.destinationExists = this.RandomNavMeshPoint(this.transform.position, direction.normalized, this.dodgeMinLookAhead, this.dodgeRadius, out this.randomDestination);

            if (this.destinationExists)
            {
                Debug.LogWarning("Dodge!");
                var result = AIUtils.GoToLocation(this.randomDestination, ref this.aiState, this.agent, this.minDistance, Color.blue);
                if (result == Node.Status.Success) this.idolInfo.IsDodging();
                return result;
            }

            Debug.LogWarning("Can not find a way to dodge!");
            return Node.Status.Failure;
        }

        public Node.Status CheckUseSkill()
        {
            if (!this.skillManager.HaveActiveNormalSkill() && ManagersSOHolder.DifficultySettingsSO.GerNormalRandomNum() < ManagersSOHolder.DifficultySettingsSO.GetNormalSkillProbability())
                return Node.Status.Success;
            return Node.Status.Failure;
        }

        public Node.Status UseSkill()
        {
            if ((int)ManagersSOHolder.GameStateSO.CurrentGameState != idolInfo.RoomNumber)
            {
                this.currentcastingskill?.SkillSO.StopSkill();
                this.skillWasActivated = false;
                this.currentcastingskill = null;
                return Node.Status.Failure;
            }

            this.currentcastingskill = this.skillManager.RequestNormalSkill();
            if (this.currentcastingskill == null) return Node.Status.Failure;

            this.skillManager.ActivateSkill(this.currentcastingskill);
            Debug.LogWarning("Start Skill");
            return Node.Status.Success;

            /*           if (!this.skillWasActivated)
                       {
                           this.currentcastingskill = this.skillManager.RequestNormalSkill();
                           if (this.currentcastingskill == null) return Node.Status.Failure;

                           this.skillManager.ActivateSkill(this.currentcastingskill);
                           this.skillWasActivated = true;
                           Debug.LogWarning("Start Skill");
                           return Node.Status.Running;
                       }
                       else if (this.skillWasActivated)
                       {
                           this.skillWasActivated = false;
                           this.currentcastingskill = null;
                           Debug.LogWarning("StopCast!");
                           return Node.Status.Success;
                       }
                       this.currentcastingskill = null;
            return Node.Status.Failure;*/
        }

        public Node.Status RunFromPaw()
        {
            this.agent.speed = this.idolInfo.RunningSpeed;
            this.agent.acceleration = this.idolInfo.RunAcceleration;
            this.idolInfo.SetRotSpeed(this.idolInfo.RunRotSpeed);

            var direction = this.transform.position - this.paw.position;

            var pawIsFar = this.PawIsFar(direction, this.ignoreDistance);

            if (aiState == AIState.Idle && pawIsFar)
            {
                Debug.LogWarning("Resting");
                if (this.idolEnergy.CurrentValue < this.idolEnergy.MaxValue && !this.idolEnergy.IsRegenerating) this.idolEnergy.SetEnergyRestore(true);
                return Node.Status.Success;
            }

            if (this.aiState == AIState.Idle)
                this.destinationExists = this.RandomNavMeshPoint(this.transform.position, direction.normalized, this.minLookAhead, this.runRadius, out this.randomDestination);

            if (this.destinationExists)
            {
                Debug.LogWarning("Run from a paw!!");
                if(this.idolEnergy.IsRegenerating) this.idolEnergy.SetEnergyRestore(false);
                return AIUtils.GoToLocation(this.randomDestination, ref this.aiState, this.agent, this.minDistance, Color.green);

            }

            Debug.LogWarning("Can not find a way to run!");
            if (this.idolEnergy.IsRegenerating) this.idolEnergy.SetEnergyRestore(false);
            this.aiState = AIState.Idle;
            return Node.Status.Failure;
        }

        bool RandomNavMeshPoint(Vector3 center, Vector3 direction, float lookAhead, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                var randomAngle = Random.Range(this.searchAngle, -this.searchAngle);
                var randomizedDirection = Quaternion.AngleAxis(randomAngle, Vector3.forward) * direction;
                Vector3 randomPoint = center + randomizedDirection * Random.Range(lookAhead, range);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, this.runRadius, NavMesh.AllAreas))
                {
                    Vector3 ret = hit.position;
                    Vector3 pathDir = center - ret;
                    ret += pathDir.normalized * (agent.radius);

                    result = ret;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        bool PawIsFar(Vector3 direction, float checkDistance) => direction.magnitude >= checkDistance;
    }
}