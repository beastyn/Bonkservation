namespace AI.Tree.Idols
{
    using Gameplay;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    public enum AIState
    {
        Idle,
        Moving
    }

    public class MischieveHelper : MonoBehaviour
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] VodKillersSO vodKillers;
        [SerializeField] float minDistance = 2f;
        [SerializeField] IdolInfoSO idolInfo;
        [SerializeField] EnergySO idolEnergy;
        [SerializeField] VodKillersSO killersSO;

        AIState aiState = AIState.Idle;
        float startMischieveTime = 0f;

        //VodKillers branch
        [SerializeField] VodKillersRotation vodKillersRotator;
        Transform targetMischieve;
        public Node.Status RequestMischieveObject()
        {
            this.targetMischieve = this.vodKillersRotator.RequestKiller();
            if (this.targetMischieve != null)
                return Node.Status.Success;
            else
                return Node.Status.Failure;

        }

        public Node.Status GoToMischieveObject()
        {
            if (this.targetMischieve == null) return Node.Status.Failure;

            this.agent.speed = this.idolInfo.CurrentWalkingSpeed;
            this.agent.acceleration = this.idolInfo.Acceleration;
            this.idolInfo.SetRotSpeed(this.idolInfo.RotSpeed);

            if (this.idolEnergy.CurrentValue < this.idolEnergy.MaxValue && !this.idolEnergy.IsRegenerating) this.idolEnergy.SetEnergyRestore(true);
            if (!this.killersSO.CanUse) this.killersSO.SetKillerUsage(true);

            return AIUtils.GoToLocation(this.targetMischieve.position, ref this.aiState, this.agent, this.minDistance, Color.red, AIUtils.SawThePaw(this.idolInfo));
        }

        public Node.Status StayAtMischieveObject()
        {
            if (AIUtils.SawThePaw(this.idolInfo))
            {
                this.aiState = AIState.Idle;
                this.startMischieveTime = 0f;
                return Node.Status.Failure;
            }

            if (this.startMischieveTime < 3f)
            {
                this.startMischieveTime += Time.deltaTime;
                if (this.idolEnergy.CurrentValue < this.idolEnergy.MaxValue && !this.idolEnergy.IsRegenerating) this.idolEnergy.SetEnergyRestore(true);
                this.aiState = AIState.Idle;
                return Node.Status.Running;
            }
            else
            {
                this.startMischieveTime = 0f;
                return Node.Status.Success;
            }
        }

        public void FailHandler() => this.killersSO.SetKillerUsage(false);

        /*Node.Status GoToLocation(Vector3 destination)
        {
            var distance = Vector3.Distance(destination, this.transform.position);
            if (this.aiState == AIState.Idle)
            {
                this.agent.speed = this.idolInfo.CurrentWalkingSpeed;
                this.agent.SetDestination(destination);
                this.aiState = AIState.Moving;
            }
            else if (Vector3.Distance(this.agent.pathEndPosition, destination) >= this.minDistance)
            {
                this.aiState = AIState.Idle;
                return Node.Status.Failure;
            }
            else if (distance < this.minDistance)
            {
                this.aiState = AIState.Idle;
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }*/
    }
}
