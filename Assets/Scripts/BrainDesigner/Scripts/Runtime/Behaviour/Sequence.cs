namespace BrainDesigner.Scripts.Runtime
{
    public class Sequence : ComplexNode
    {

        int currentChild;


        protected override void OnEnable() => this.currentChild = 0;

        protected override NodeState OnUpdate()
        {
            var child = children[this.currentChild];

            switch (child.Update())
            {
                case NodeState.Running:
                    return NodeState.Running;
                case NodeState.Failure:
                    return NodeState.Failure;
                case NodeState.Success:
                    this.currentChild++;
                    break;
            }

            return this.currentChild == children.Count ? NodeState.Success : NodeState.Running;
        }
    }

}