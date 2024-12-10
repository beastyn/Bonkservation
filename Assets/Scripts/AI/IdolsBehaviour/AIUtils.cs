namespace AI.Tree
{
    using AI.Tree.Idols;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using Unity.VisualScripting;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.AI;

    public static class AIUtils
    {
        static float cachedDistance = 0f;
        static int timesNotMoved = 0;
        public static Node.Status GoToLocation(Vector3 destination, ref AIState agentState, NavMeshAgent agent, float minDistance, Color lineColor, bool forceFail = false)
        {
            var distance = Vector3.Distance(destination, agent.transform.position);

            NavMeshPath navMeshPath = new NavMeshPath();
            //create path and check if it can be done
            // and check if navMeshAgent can reach its target
            if (agentState == AIState.Idle)
            {
                if (agent.CalculatePath(destination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    //move to target
                    agent.SetPath(navMeshPath);
                    agentState = AIState.Moving;
                    Debug.DrawLine(agent.transform.position, destination, lineColor, 2f);
                }
                else
                {
                    agentState = AIState.Idle;
                    Debug.DrawLine(agent.transform.position, agent.pathEndPosition, Color.white, 2f);
                    Debug.LogWarning($"Failed go to location with distance{Vector3.Distance(agent.pathEndPosition, destination)}");
                    return Node.Status.Failure;
                }
            }
            if(agentState == AIState.Moving && forceFail)
            {
                agentState = AIState.Idle;
                Debug.DrawLine(agent.transform.position, agent.pathEndPosition, Color.white, 2f);
                Debug.LogWarning($"Interrupted!");
                return Node.Status.Failure;
            }


            if(cachedDistance == distance)
            {
                timesNotMoved++;
                if (timesNotMoved > 10)
                {
                    agentState = AIState.Idle;
                    cachedDistance = 0f;
                    timesNotMoved = 0;
                    Debug.DrawLine(agent.transform.position, agent.pathEndPosition, Color.white, 2f);
                    Debug.LogWarning($"Stuck!");
                    return Node.Status.Failure;
                }
            }
            else
                cachedDistance = distance;

            if (distance < minDistance && agentState == AIState.Moving)
            {
                agentState = AIState.Idle;
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        public static bool SawThePaw(IdolInfoSO idolInfo)
        {
            if (idolInfo.RoomNumber == (int)ManagersSOHolder.GameStateSO.CurrentGameState)
                return true;
            return false;
        }
    }
}
