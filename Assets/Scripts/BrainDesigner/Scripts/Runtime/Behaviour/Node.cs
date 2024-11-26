using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using BrainDesigner.Scripts.Utils;
using UnityEngine.UIElements;

namespace BrainDesigner.Scripts.Runtime
{
    /// <summary> Atomic level of a behavior entity. It can be used as action, as sequence or behavior itself as includes all minimum requirements</summary>

    [Serializable]
    public abstract class Node: Named
    {
        public enum NodeState
        {
            Success,
            Running,
            Failure,
            Disabled
        } 
        public string Description { get; }

        protected GameObject AgentObject => this.agentObject;
        protected SceneReferences SceneRefs => this.brainDesigner?.sceneReferences;

        internal NodeState State => this.nodeState;
        internal bool Started => this.started;
        internal bool Enabled => this.enabled;

        internal string GUID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }

        internal Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        [NonSerialized] NodeState nodeState = NodeState.Disabled;
        [NonSerialized] bool started;
        [NonSerialized] bool enabled;

        [SerializeField] string guid; //Unique id.
        [SerializeField] Vector2 position;


        [NonSerialized] BrainDesigner brainDesigner;
        [NonSerialized] GameObject agentObject;
        //designer inpsector
        [NonReorderable] VisualElement nodeInspectorContent;

        internal void InitializeNode(BrainDesigner brainDesigner = null, GameObject thisGameObject = null)
        {
            this.started = false;
            this.enabled = false;
            this.agentObject = thisGameObject;
            this.brainDesigner = brainDesigner;
        }

        internal NodeState Update()
        {
            if (!started)
            {
                OnAwake();
                started = true;
            }

            if (!enabled)
            {
                OnEnable();
                enabled = true;
                nodeState = NodeState.Running;
            }

            nodeState = OnUpdate();

            if (nodeState != NodeState.Running)
            {
                OnDisable();
                enabled = false;
            }

            return nodeState;
        }

        internal virtual void Disable()
        {
            if (nodeState == NodeState.Running)
            {
                OnDisable();
                enabled = false;
            }

            nodeState = NodeState.Disabled;
        }
        protected virtual void OnAwake() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected abstract NodeState OnUpdate();

        /* protected abstract List<KeyValuePair<string,Type>> RequestedVariables ();
         protected virtual void RegisterSerializedVariables() 
         {
             foreach (var pair in this.RequestedVariables())
             { 
                 this.AddVariable(pair.Key, pair.Value);
             }
         }

         /// <summary>
         /// Creates and adds a new dropdown to the node inspector based on the provided parameters.
         /// </summary>
         /// <typeparam name="T">The type of the variable.</typeparam>
         /// <param name="name">The name of the variable.</param>
         /// <param name="variable">The variable, whose value is to be serialized.</param>
         protected void AddVariable<T>(string name, T variable)
         {
             string value;

             if (variable is Component or BehaviourSet)
             {
                 value = variable switch
                 {
                     Component component => component.gameObject.name,
                     BehaviourSet set => set.name,
                     _ => ""
                 };
             }
             else if (this.IsPrimitiveDataType<T>())
                 value = variable == null ? "" : variable.ToString();
             else
                 value = ConvertUnityTypeToString(variable);

             string readableName = Utils.Utils.VariableNameToReadable(name);

             if (_details != null && _details.Contains(readableName))
             {
                 string pattern = $@"({readableName}:\s)([^\n]+)";
                 string replacement = $"{readableName}: <b>{value}</b>";

                 _details = Regex.Replace(_details, pattern, replacement);
                 return;
             }

             _details = _details == null
                 ? string.Concat(_details, $"{readableName}: <b>{value}</b>")
                 : string.Concat(_details, "\n", $"{readableName}: <b>{value}</b>");
         }

         private string ConvertUnityTypeToString(object obj)
         {
             if (obj == null)
                 return "";

             return obj switch
             {
                 Vector2 vector2 => $"({vector2.x}, {vector2.y})",
                 Vector3 vector3 => $"({vector3.x}, {vector3.y}, {vector3.z})",
                 Vector4 vector4 => $"({vector4.x}, {vector4.y}, {vector4.z}, {vector4.w})",
                 Quaternion quaternion => $"({quaternion.x}, {quaternion.y}, {quaternion.z}, {quaternion.w})",
                 Color color => $"({color.r}, {color.g}, {color.b}, {color.a})",
                 Bounds bounds => $"(Center: {bounds.center}, Size: {bounds.size})",
                 Rect rect => $"(Position: {rect.position}, Size: {rect.size})",
                 LayerMask layerMask => LayerMask.LayerToName(layerMask.value),
                 _ => obj.ToString()
             };
         }

         private bool IsPrimitiveDataType<T>()
         {
             Type type = typeof(T);
             return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
         }*/

        /// <summary>
        /// Registers custom dropdowns to be displayed in the node inspector, by using the "AddDropdown" method.
        /// </summary>
        protected virtual void RegisterDropdowns() { }

        /// <summary>Add a custom dropdown to an editor.</summary>

        protected void AddDropdown<T>(string nameInInspector, List<T> listType, int selectedIndex, Action<int> onSelectedIndexChanged)
        {
            if (this.nodeInspectorContent == null)
                return;

            if (listType == null)
            {
                var helpBox = new HelpBox($"Add a list of type <b>{typeof(T).Name}</b> to SceneReferences", HelpBoxMessageType.Warning);
                this.nodeInspectorContent.Add(helpBox);
                return;
            }

            if (listType.Count == 0)
            {
                var helpBox = new HelpBox($"The list of type <b>{typeof(T).Name}</b> in SceneReferences is empty", HelpBoxMessageType.Info);
                this.nodeInspectorContent.Add(helpBox);
                return;
            }

            var dropdown = new PopupField<T>(nameInInspector, listType, selectedIndex,
                item => item is Component component ? component.gameObject.name :
                    item is Behaviour beh ? beh.Name :
                    item != null ? item.ToString() : "None",
                item => item is Component component ? component.gameObject.name :
                    item is BehaviourSet beh ? beh.Name :
                    item != null ? item.ToString() : "None");

            dropdown.RegisterValueChangedCallback(evt =>
            {
                onSelectedIndexChanged?.Invoke(listType.IndexOf(evt.newValue));
            });
            this.nodeInspectorContent.Add(dropdown);
        }

        internal void InitializedDropdowns(VisualElement inspectorContent)
        {
            this.nodeInspectorContent = inspectorContent;

            if (this.SceneRefs != null)
                RegisterDropdowns();
            else
            {
                var helpBox = new HelpBox("Scene References missing.", HelpBoxMessageType.Info);
                var spacer = new VisualElement
                {
                    style =
                    {
                        height = 5
                    }
                };
                this.nodeInspectorContent.Add(helpBox);
                this.nodeInspectorContent.Add(spacer);
            }
        }

    }
}
