namespace AI.Tree
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Leaf : Node
    {
        public delegate Status Tick();
        public Tick ProcessMethod;

        public Leaf() { }

        public Leaf(string name, Tick processMethod)
        {
            this.Name= name;
            this.ProcessMethod = processMethod;
        }

        public override Status Process()
        {
            if (this.ProcessMethod != null)
                return this.ProcessMethod();
            else
            {

                Debug.LogWarning($"{this.Name} Failed");
                return Status.Failure;
            }
        }
    }
}