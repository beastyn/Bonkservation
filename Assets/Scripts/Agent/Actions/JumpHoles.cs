using UnityEngine;
using BrainDesigner.Scripts.Runtime;
using UnityEngine.AI;

public class JumpHoles : Action
{
    protected override bool NeedSceneRefs => true;

    [SerializeField] int transformIndex;

    Transform jumpObjectsParent;
    int jumpObjectsCount;
    Vector3 positionToJump;
    NavMeshAgent navMeshAgent;
    Vector3 scale;

    protected override void RegisterDropdowns()
    {
        base.RegisterDropdowns();
        AddDropdown("Parent for Jumscare hides", SceneRefs.GetListOfType<Transform>(), this.transformIndex,
          newIndex => { this.transformIndex = newIndex; });
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        this.navMeshAgent = this.AgentObject.GetComponent<NavMeshAgent>();
        this.jumpObjectsParent = this.SceneRefs.GetRef<Transform>(this.transformIndex);
        this.scale = this.AgentObject.transform.localScale;

        if (this.jumpObjectsParent == null && this.navMeshAgent == null)
            this.ReferenceMissing = true;
    }

    protected override void OnEnable()
    {
        if (this.ReferenceMissing)
            return;

        this.navMeshAgent.speed = 0f;
        this.jumpObjectsCount = this.jumpObjectsParent.childCount;
        var objectToJump = this.jumpObjectsParent.GetChild(Random.Range(0, this.jumpObjectsCount));
        this.positionToJump = new Vector3(objectToJump.position.x, this.AgentObject.transform.position.y, objectToJump.position.z);
    }

    protected override NodeState OnUpdate()
    {
        if (this.jumpObjectsCount <= 0)
            return NodeState.Failure;

        if ((this.AgentObject.transform.position - this.positionToJump).magnitude > 0.01)
        {
            this.AgentObject.transform.localScale = Vector3.zero;
            this.AgentObject.transform.position = this.positionToJump;
            this.AgentObject.transform.localScale = this.scale;
            return NodeState.Running;
        }

        return NodeState.Success;

    }

    protected override void OnDisable()
    {

    }
}
