namespace AI.Tree
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Selector : Node
    {
        public Selector(string name) => this.Name = name;

        public override Status Process()
        {
            var childStatus = this.Children[this.CurrentChild].Process();

            if (childStatus == Status.Running)
                return Status.Running;

            if (childStatus == Status.Success)
            {
                this.CurrentChild = 0;
                return Status.Success;
            }

            this.CurrentChild++;


            if (this.CurrentChild >= this.Children.Count)
            {
                this.CurrentChild = 0;
                return Status.Failure;
            }
            Debug.LogWarning($"{this.Name} went to a {this.Children[this.CurrentChild].Name}");
            return Status.Running;
        }
    }
}
