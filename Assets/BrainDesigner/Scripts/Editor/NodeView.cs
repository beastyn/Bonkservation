#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace BrainDesigner.Scripts.Editor
{
    using Utils;
    using Runtime;

    public class NodeView :  UnityEditor.Experimental.GraphView.Node
    {
        internal Action<NodeView> onNodeSelected;
        internal readonly Node node;
        internal Port input;
        internal Port output;

        readonly Label labelDetails;
        private readonly VisualElement titlePlace;


        internal NodeView(Node baseNode) : base(LoadUxml())
        {
            node = baseNode;
            title = title = Utils.AddSpacesBeforeUppercase(node.GetType().Name);
            viewDataKey = node.GUID;

            style.left = node.Position.x;
            style.top = node.Position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();

            this.labelDetails = this.Q<Label>("details-label");
            this.titlePlace = this.Q<VisualElement>("title");
            UpdateNodeDetails();
        }

        static string LoadUxml()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            MonoScript scriptAsset = MonoImporter.GetAllRuntimeMonoScripts()
                .Where(script => script != null && script.GetClass() != null)
                .FirstOrDefault(script => script.GetClass().Assembly == assembly);

            string scriptPath = AssetDatabase.GetAssetPath(scriptAsset);
            string scriptsDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(scriptPath)));

            return scriptsDirectory != null ? Path.Combine(scriptsDirectory, "Scripts/Editor/UXML/NodeView.uxml") : null;
        }

        private void UpdateNodeDetails()
        {
            /*string text = node.GetSerializedValue();
            if (text == null)
            {
                this.labelDetails.parent.style.display = DisplayStyle.None;
                _titleContainer.style.paddingTop = 4;
                _titleContainer.style.paddingBottom = 4;
            }
            else
                _labelDetails.text = node.GetSerializedValue();*/
            this.labelDetails.text = node.GUID;
        }

        public sealed override string title
        {
            get => base.title;
            set => base.title = value;
        }

        private void SetupClasses()
        {
            switch (node)
            {
                case Action:
                    AddToClassList("action");
                    break;               
                case ComplexNode:
                    AddToClassList("complex");
                    break;
            }
        }

        private void CreateInputPorts()
        {
            switch (node)
            {
                case Action:
                case ComplexNode:
                    input = new NodePort(Direction.Input, Port.Capacity.Single);
                    break;
            }

            if (input == null)
                return;

            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);

            inputContainer.style.height = 14f;
            inputContainer.style.flexDirection = FlexDirection.Row;
        }

        private void CreateOutputPorts()
        {
            switch (node)
            {
                case Action:
                case ComplexNode:
                    output = new NodePort(Direction.Output, Port.Capacity.Multi);
                    break;
               
            }

            if (output == null)
                return;

            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);

            outputContainer.style.height = 14f;
            outputContainer.style.flexDirection = FlexDirection.Row;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            node.Position = new Vector2(newPos.xMin, newPos.yMin);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            onNodeSelected?.Invoke(this);
            UpdateNodeDetails();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            onNodeSelected?.Invoke(null);
            UpdateNodeDetails();
        }

        internal void SortChildren()
        {
            if (node is ComplexNode complex)
                complex.children.Sort(SortByHorizontalPosition);
        }

        private int SortByHorizontalPosition(Node left, Node right)
        {
            return left.Position.x < right.Position.x ? -1 : 1;
        }

        internal void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            switch (node.State)
            {
                case Node.NodeState.Running:
                    if (node.Enabled)
                        AddToClassList("running");
                    break;
                case Node.NodeState.Failure:
                    AddToClassList("failure");
                    break;
                case Node.NodeState.Success:
                    AddToClassList("success");
                    break;
                case Node.NodeState.Interrupted:
                    AddToClassList("interrupted");
                    break;
            }
        }
    }
}
#endif
