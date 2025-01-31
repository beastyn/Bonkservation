using UnityEditor.Animations;

namespace BrainDesigner.Scripts.Runtime
{
    public class Sequence : ComplexNode
    {
        internal Action RunningAction { get; private set; }
        int currentChild;


        protected override void OnEnable() => this.currentChild = 0;

        protected override NodeState OnUpdate()
        {
            var child = children[this.currentChild];

            switch (child.Update())
            {
                case NodeState.Running:
                    {
                        if (child is Action) this.RunningAction = (Action)child;
                        return NodeState.Running;
                    }
                case NodeState.Failure:
                    {
                        if (child is Action) this.RunningAction = null;
                        return NodeState.Failure;
                    }
                case NodeState.Success:
                    this.currentChild++;
                    break;
            }
            var isLastChild = this.currentChild == children.Count;
            if (isLastChild && child is Action) this.RunningAction = null;

            return isLastChild  ? NodeState.Success : NodeState.Running;
        }
    }

}