using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;

public class WakeUp : Action
{
    protected override bool NeedSceneRefs => false;

    public float WakeupTime = 1f;

    NavMeshAgent navMeshAgent;

    float startTime;
    float currentTime;

    protected override void OnAwake()
    {
        base.OnAwake();
        this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();

        if (this.navMeshAgent == null)
            this.ReferenceMissing = true;
    }

    protected override void OnEnable()
    {

        if (this.ReferenceMissing)
            return;

        this.navMeshAgent.speed = 0f;
        this.startTime = Time.realtimeSinceStartup;

    }

    protected override NodeState OnUpdate()
    {

        if (this.ReferenceMissing)
            return NodeState.Failure;


        this.currentTime = Time.realtimeSinceStartup;

        if (this.currentTime - this.startTime < this.WakeupTime)
            return NodeState.Running;

        return NodeState.Success;
    }

    protected override void OnDisable()
    {
        this.startTime = 0f;
        this.currentTime = 0f;
    }

    protected override void OnInterrupt() => this.OnDisable();
}
