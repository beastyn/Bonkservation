namespace AI
{
    using AI.Tree;
    using Gameplay;
    using Managers;
    using Managers.Timer;
    using NavMeshPlus.Extensions;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    public class RestingHelper : MonoBehaviour
    {
        [SerializeField] EnergySO idolEnergy;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] IdolInfoSO idolInfo;

        float timeResting = 0f;

        public Node.Status CheckIfNeedRest()
        {
            if (this.idolEnergy.CurrentValue == 0)
                return Node.Status.Success;
            return Node.Status.Failure;
        }

        public Node.Status Rest()
        {
            this.agent.speed = 0f;
            this.timeResting += Time.deltaTime;
            if (this.timeResting <= 10f || idolInfo.RoomNumber == (int)ManagersSOHolder.GameStateSO.CurrentGameState)
                return Node.Status.Running;
            else
            {
                this.timeResting = 0f;
                return Node.Status.Failure;
            }
        }
    }
}
