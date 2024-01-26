namespace AI.Tree
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Sequence : Node
    {
        public Sequence(string name) => this.Name = name;

        public Sequence(string name, FailureCallback failureCallback)
        {
            this.Name = name;
            this.FailureMethod = failureCallback;
        }

        public override Status Process()
        {
            var childStatus = this.Children[this.CurrentChild].Process();

            if (childStatus == Status.Running)
                return Status.Running;

            if (childStatus == Status.Failure)
            {
                Debug.LogWarning($"{this.Name} Failed");
                this.CurrentChild = 0;
                if(this.FailureMethod!= null) this.FailureMethod();
                return Status.Failure;
            }

            this.CurrentChild++;

            if(this.CurrentChild >=this.Children.Count)
                this.CurrentChild = 0;
            return Status.Success;

        }
    }
}
