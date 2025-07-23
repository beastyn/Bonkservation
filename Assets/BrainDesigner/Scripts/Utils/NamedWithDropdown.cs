using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using BrainDesigner.Scripts.Runtime;

using Behaviour = BrainDesigner.Scripts.Runtime.Behaviour;

namespace BrainDesigner.Scripts.Utils
{
    [Serializable]
    public abstract class NamedWithDropdown
    {
        [SerializeField] string name;

        internal string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        protected abstract SceneReferences SceneRefs { get; }

        //designer inpsector
        [NonReorderable] VisualElement objectInspectorContent;

        protected virtual void RegisterDropdowns() { }

        /// <summary>Add a custom dropdown to an editor.</summary>

        protected void AddDropdown<T>(string nameInInspector, List<T> listType, int selectedIndex, Action<int> onSelectedIndexChanged)
        {
            if (this.objectInspectorContent == null)
                return;

            if (listType == null)
            {
                var helpBox = new HelpBox($"Add a list of type <b>{typeof(T).Name}</b> to SceneReferences", HelpBoxMessageType.Warning);
                this.objectInspectorContent.Add(helpBox);
                return;
            }

            if (listType.Count == 0)
            {
                var helpBox = new HelpBox($"The list of type <b>{typeof(T).Name}</b> in SceneReferences is empty", HelpBoxMessageType.Info);
                this.objectInspectorContent.Add(helpBox);
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
            this.objectInspectorContent.Add(dropdown);
        }

        internal void InitializedDropdowns(VisualElement inspectorContent)
        {
            this.objectInspectorContent = inspectorContent;

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
                this.objectInspectorContent.Add(helpBox);
                this.objectInspectorContent.Add(spacer);
            }
        }
    }
}
