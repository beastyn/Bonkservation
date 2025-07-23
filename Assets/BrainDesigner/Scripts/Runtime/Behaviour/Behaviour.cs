using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace BrainDesigner.Scripts.Runtime
{
    using Utils;

    [Serializable]
    public class Behaviour: NamedWithDropdown
    {
        ////[SerializeField] internal List<Precondition> preconditions = new();
        //[SerializeField] internal List<Evaluator> evaluators = new();
        //[SerializeField] internal Sequence actionSequence = new();
        internal List<Task> ActivationTasks => this.activationTask;
        internal Node.NodeState State => this.behaviourSequence.State;

        protected override SceneReferences SceneRefs => this.brainDesigner?.sceneReferences;

        [SerializeField] internal bool active;
        [SerializeField] internal bool isDefault;
        [SerializeField] internal bool usesPhysics = false;

        [SerializeField] internal bool critical;
        [SerializeField] internal float baseScore=0f;
        [SerializeReference] internal Sequence behaviourSequence;
 
        [SerializeReference] List<Task> activationTask = new();
        [NonSerialized] BrainDesigner brainDesigner;
        internal List<Node> nodes = new();

        /*        [SerializeField] internal string designation;
                [SerializeField] internal float weight;
                [SerializeField] internal float executionFactor;*/
        
        /*        [SerializeField] internal bool setMinScore;
                [SerializeField] internal float minScore;
                [SerializeField] internal bool setMaxScore;
                [SerializeField] internal float maxScore;
                [SerializeField] internal float failChance;
                [SerializeField] internal string notes;*/


        public bool IsValidToStart() => true;

        internal void Initialize(BrainDesigner brainDesigner, GameObject thisAgent)
        {
            //preconditions.ForEach(precondition => precondition.Initialize(utilityDesigner));
            //evaluators.ForEach(evaluator => evaluator.Initialize(utilityDesigner));
            //lastScore = 0;
            this.brainDesigner = brainDesigner;
            if (this.behaviourSequence == null)
                return;

            this.InitializeNodeList();

            foreach (var node in this.nodes)
                node.InitializeNode(brainDesigner, thisAgent);
        }

        internal void TickExecution()
        {
            this.behaviourSequence.Update();
        }

        internal void Interrupt() => this.behaviourSequence.Interrupt();
        void InitializeNodeList()
        {
            this.nodes.Add(this.behaviourSequence);
            this.nodes.AddRange(this.behaviourSequence.GetAllChildren());
        }

/*        internal List<Node> GetAllNodes(Node startNode)
        {
            List<Node> nodesList = new();

            if (startNode != null)
                this.TraverseNodes(startNode, nodesList);

            return nodesList;
        }*/

/*        void TraverseNodes(Node currentNode, ICollection<Node> nodesList)
        {
            nodesList.Add(currentNode);
            Debug.Log($"Mode Added: {currentNode.name}");

            switch (currentNode)
            {                
                case ComplexNode complexNode:
                    foreach (Node childNode in complexNode.children)
                        TraverseNodes(childNode, nodesList);
                    break;
            }
        }*/

#if UNITY_EDITOR
        Node CreateNodeInstance(Type type)
        {
            if (Activator.CreateInstance(type) is not Node node)
                return null;

            node.GUID = GUID.Generate().ToString();
            return node;

        }

        internal Node CreateNode(Type type, Vector2 pos, bool isRootSequence = false)
        {
            Node node = CreateNodeInstance(type);
            node.Position = pos;
            if(!isRootSequence) this.AddChild(this.behaviourSequence,node);
            nodes.Add(node);
            return node;
        }

        internal void DeleteNode(Node node)
        {
            nodes.Remove(node);

            AssetDatabase.SaveAssets();
        }

        internal void AddChild(Node parent, Node child)
        {
            switch (parent)
            {
                case ComplexNode complex:
                    {
                        complex.children.Add(child);
                        break;
                    }
            }
        }

        internal void RemoveChild(Node parent, Node child = null)
        {
            switch (parent)
            {
                case ComplexNode complex when complex.children.Contains(child):
                    complex.children.Remove(child);
                    break;
            }
        }
#endif
        internal List<Node> GetChildren(Node parent)
        {
            List<Node> children = new();

            switch (parent)
            {
                case ComplexNode complex:
                    return complex.children;
            }

            return children;
        }

        public object Clone()
        {
            Type type = this.GetType();
            Behaviour newBehaviour = (Behaviour)Activator.CreateInstance(type);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo[] fields = type.GetFields(flags);

            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(this);
                object copiedValue = Utils.DeepCopy(fieldValue);
                field.SetValue(newBehaviour, copiedValue);
            }

            return newBehaviour;
        }

    }
}
