using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrainDesigner.Scripts.Utils
{
    internal class SetUtils
    {
        internal struct SetData<T> where T : Named
        {
            internal VisualElement parentVisualElement;
            internal ListView listView;
            internal VisualTreeAsset templateElement;
            internal float elementHeight;
            internal Set<T> setToLoad;
            internal Dictionary<T, VisualElement> typeToElement;
            internal Named selectedElement;
            internal SelectionType selectionType;

            internal SetData(VisualElement parentVisualElement, ListView listView, VisualTreeAsset templateElement, float elementHeight, Set<T> setToLoad, Dictionary<T, VisualElement> typeToElement, Named selectedElement, SelectionType selectionType)
            {
                this.parentVisualElement = parentVisualElement;
                this.listView = listView;
                this.templateElement = templateElement;
                this.elementHeight = elementHeight;
                this.setToLoad = setToLoad;
                this.typeToElement = typeToElement;
                this.selectedElement = selectedElement;
                this.selectionType = selectionType;
            }
        }

        internal static SetData<T> CreateSetList<T>(SetData<T> setData, Action<IEnumerable<object>> LoadElement, VisualElement additionalInfoContainer = null, bool defaultSet = true) where T : Named
        {
            Action currentRemoveButtonAction = null;
            setData.parentVisualElement.Clear();

            VisualElement CreateElement() => setData.templateElement.Instantiate();

            void OnButtonRemoveElementClick<T>(SetData<T> setData, Action<IEnumerable<object>> LoadElement, VisualElement additionalInfoContainer, int i, T elementReference) where T : Named
            {
                if (!EditorUtility.DisplayDialog("Confirmation",
                        $"Are you sure you want to remove this {elementReference.GetType().Name}?",
                        "Yes", "No"))
                    return;

                setData.setToLoad.RemoveSetElementAt(i);

                if (setData.setToLoad.list.Count <= 0)
                {
                    setData.selectedElement = null;
                    if (additionalInfoContainer != null) additionalInfoContainer.style.visibility = Visibility.Hidden;
                }
                else
                {
                    int nextElementIndex = setData.setToLoad.list.Count > i ? i : i - 1;
                    LoadElement?.Invoke(new List<Named> { setData.setToLoad.list[nextElementIndex] });
                    setData.listView.SetSelection(nextElementIndex);
                }

                setData.listView.Rebuild();
            }


            //additional method €˚Ù‚Ù˚


            void BindElement(VisualElement e, int i)
            {
                var elementReference = setData.setToLoad.list[i];

                if (elementReference is not Named && elementReference is not T)
                {
                    Utils.ShowHelpBoxInEditor(setData.parentVisualElement, "Type of element is not Named");
                    return;
                }

                setData.typeToElement[(T)elementReference] = e.Q<VisualElement>("Container");

                e.Q<Label>("LabelTitle").text = elementReference.Name;

                if (!defaultSet) return;

                e.AddToClassList("list-view-item");

                currentRemoveButtonAction ??= () => OnButtonRemoveElementClick(setData, LoadElement, additionalInfoContainer, i, elementReference);
                var removeButton = e.Q<Button>("ButtonRemoveElement");
                removeButton.clickable.clicked -= currentRemoveButtonAction;
                removeButton.clickable.clicked += currentRemoveButtonAction;
            }

            setData.listView = new ListView(setData.setToLoad.list, setData.elementHeight, CreateElement, BindElement) { selectionType = setData.selectionType };
            setData.listView.selectionChanged += LoadElement;
            setData.parentVisualElement.Add(setData.listView);

            return setData;
        }

        
        internal static void LoadSetDropdown<T>(ToolbarMenu menuSet, List<int> listIdsOrdered, Dictionary<int, T> sets, Action<int> loadAction) where T:Named
        {
            menuSet.menu.ClearItems();
            listIdsOrdered.Clear();

            foreach (var sensorSet in sets)
            {
                menuSet.menu.AppendAction(sensorSet.Value.Name, action => loadAction(sensorSet.Key));
                listIdsOrdered.Add((int)sensorSet.Key);
            }
        }
    }
}