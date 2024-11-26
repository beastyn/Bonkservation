#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrainDesigner.Scripts.Editor
{
    using Utils;
    using Runtime;

    [UxmlElement]
    public partial class BehaviourEditView : GraphView
    {
        internal Action<NodeView> onNodeSelected;

        Behaviour behaviour;
        NodeView  behaviorSequenceNodeView;
        BrainDesigner brainDesigner;
        BrainDesignerEditorWindow brainDesignerEditorWindow;


        public BehaviourEditView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("BehaviourEditView"));

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.GUID) as NodeView;
        }

        internal void PopulateView(Behaviour behaviour, BrainDesigner brainDesigner, BrainDesignerEditorWindow brainDesignerEditorWindow)
        {
            if (behaviour == null || brainDesigner == null)
                return;

            this.behaviour = behaviour;
            this.brainDesigner = brainDesigner;
            this.brainDesignerEditorWindow = brainDesignerEditorWindow;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            //Populate roo sequence node.
            this.behaviour.behaviourSequence ??= this.behaviour.CreateNode(typeof(Sequence), new Vector2(375, 10), true) as Sequence;
            this.behaviorSequenceNodeView = this.CreateNodeView(behaviour.behaviourSequence, true);

            this.TraverseAnDrawPorts(this.behaviour.behaviourSequence);
        }
       

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(
                endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(element =>
            {
                switch (element)
                {
                   /* case NodeView nodeView:
                        this.behaviour.RemoveChild(behaviour,nodeView.node);
                        break;*/
                    case Edge edge:
                        {
                            if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                                this.behaviour.RemoveChild(parentView.node, childView.node);
                            break;
                        }
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                    this.behaviour.AddChild(parentView.node, childView.node);
            });

            if (graphViewChange.movedElements != null)
            {
                nodes.ForEach(n =>
                {
                    NodeView nodeView = n as NodeView;

                    nodeView?.SortChildren();
                });
            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            Vector2 nodePos = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            AddItems<Action>(evt.menu, nodePos, "Actions");
            AddItems<ComplexNode>(evt.menu, nodePos, "Complex");
        }

        internal void CreateContextualMenu(Vector2 position)
        {
            GenericMenu menu = new GenericMenu();
            AddItems<Action>(menu, position, "Actions");
            AddItems<ComplexNode>(menu, position, "Complex");
            menu.ShowAsContext();
        }

        internal void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                NodeView view = n as NodeView;
                view?.UpdateState();
            });
        }

        void AddItems<T>(DropdownMenu menu, Vector2 position, string menuCategory) where T : class
        {
            var types = TypeCache.GetTypesDerivedFrom<T>();
            foreach (var type in types)
            {
                var isUtilityDesignerAssembly = type.Assembly.GetName().Name == "UtilityDesigner";
                var path = isUtilityDesignerAssembly
                    ? $"{menuCategory}/{Utils.AddSpacesBeforeUppercase(type.Name)}"
                    : $"{menuCategory} (custom)/{Utils.AddSpacesBeforeUppercase(type.Name)}";
                menu.AppendAction(path, (a) => CreateNode(type, position));
            }
        }

        void AddItems<T>(GenericMenu menu, Vector2 position, string menuCategory) where T : class
        {
            var types = TypeCache.GetTypesDerivedFrom<T>();
            foreach (var type in types)
            {
                var isBrainDesignerAssembly = type.Assembly.GetName().Name == "BrainDesigner";
                var path = isBrainDesignerAssembly
                    ? $"{menuCategory}/{Utils.AddSpacesBeforeUppercase(type.Name)}"
                    : $"{menuCategory} (custom)/{Utils.AddSpacesBeforeUppercase(type.Name)}";
                menu.AddItem(new GUIContent(path), false, () => CreateNode(type, position));
            }
        }

        /// <summary>Visual creation of a node, that will also follow with default connection to a root sequence</summary>
        Node CreateNode(Type type, Vector2 pos, bool isRootSequence = false)
        {
            Node node = this.behaviour.CreateNode(type, pos, isRootSequence);
            CreateNodeView(node, isRootSequence, true);
            return node;
        }

        NodeView CreateNodeView(Node node, bool isRootSequence = false, bool isManual = false)
        {
            if (!Application.isPlaying)
                node.InitializeNode(this.brainDesigner);

            NodeView nodeView = new NodeView(node)
            {
                onNodeSelected = onNodeSelected
            };
            AddElement(nodeView);
            if(!isRootSequence && isManual) AddElement(this.behaviorSequenceNodeView.output.ConnectTo(nodeView.input));

            return nodeView;
        }

        void TraverseAnDrawPorts(Node parentNode)
        {
            switch (parentNode)
            {
                case ComplexNode complex:
                    foreach (var child in complex.children)
                    {
                        this.CreateNodeView(child);

                        NodeView parentView = FindNodeView(parentNode);
                        NodeView childView = FindNodeView(child);
                        AddElement(parentView.output.ConnectTo(childView.input));
                        this.TraverseAnDrawPorts(child);
                    }
                    break;
            }

        }
    }
}
#endif