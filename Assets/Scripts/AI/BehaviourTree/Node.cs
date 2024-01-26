namespace AI.Tree
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Node

    {
        public enum Status
        {
            Success,
            Running,
            Failure
        }

        public Status State;
        public List<Node> Children = new();
        public int CurrentChild = 0;
        public string Name;

        public delegate void FailureCallback();
        public FailureCallback FailureMethod;

        public Node() { }
        public Node(string name) => this.Name = name;
        public Node(string name, FailureCallback failureCallback)
        {
            this.Name = name;
            this.FailureMethod = failureCallback;
        }

        public void AddChild(Node child) => this.Children.Add(child);

        public virtual Status Process()
        {
            return this.Children[this.CurrentChild].Process();
        }
    }
}