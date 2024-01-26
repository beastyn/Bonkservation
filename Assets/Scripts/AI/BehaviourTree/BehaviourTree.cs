namespace AI.Tree
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BehaviourTree : Node
    {
        public BehaviourTree() => this.Name = "Tree";
        public BehaviourTree(string name) => this.Name = name;

        public void PrintTree()
        {
            PrintNode(this, 0);
        }

        void PrintNode(Node node, int level)
        {
            if (node.Children == null)
                return;

            var msg = "";
            level++;
            for (var i = 0; i < level; i++)
            {
                msg += "-";
            }
            //Debug.Log($"{msg} {node.Name}\n");

            foreach (var child in node.Children)
                this.PrintNode(child, level);
        }

        public override Status Process()
        {
            return this.Children[this.CurrentChild].Process();
        }
    }
}
