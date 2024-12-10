using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts.Runtime
{
    public abstract class ComplexNode : Node
    {
        [SerializeReference] internal List<Node> children = new();

        internal override void Interrupt()
        {
            foreach (var child in children)
                child.Interrupt();
            base.Interrupt();
        }

        /// <summary>Disable every children if node is terminated.</summary>
        internal override void Disable()
        {
            foreach (var child in children)
                child.Disable();
            base.Disable();
        }

        internal List<Node> GetAllChildren()
        { 
            var allChildren = new List<Node>();
            this.TraverseThroughNodes(this, allChildren);
            return allChildren;
        }

        internal void TraverseThroughNodes(Node parentNode, List<Node> nodesList)
        {
            switch (parentNode)
            {
                case ComplexNode complex:

                    foreach (Node childNode in complex.children)
                    {
                        nodesList.Add(childNode);
                        this.TraverseThroughNodes(childNode, nodesList);
                    }
                    break;
            }
           
        }
    }
}
