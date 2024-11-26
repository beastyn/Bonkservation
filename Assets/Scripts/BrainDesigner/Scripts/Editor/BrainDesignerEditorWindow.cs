#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Android.Gradle;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BrainDesigner.Scripts.Editor
{
    using Utils;
    using Runtime;
    using System.Globalization;
    using static UnityEditor.PlayerSettings;
    using Codice.CM.SEIDInfo;
    using System.Runtime.CompilerServices;
    using Codice.Client.BaseCommands.Download;
    using Codice.Client.Common.FsNodeReaders;

    public class BrainDesignerEditorWindow : EditorWindow
    {
        static readonly Vector2 windowMinSize = new(1350, 800);
        float elementInSetHeight = 35;
        float elementInTaskLinked = 200;

        static BrainDesigner brainDesigner;
        VisualElement specificsContainer;
        VisualTreeAsset templateElementInSet;

        UnityEditor.Editor editor;
        TextField textFieldFileName;

        // Behaviour Sets
        VisualElement behaviourSetPanel;
        ToolbarMenu menuBehaviourSets;
        readonly List<int> behavioureSetIdsOrdered = new();

        VisualElement behavioursContent;
        ListView behavioursListView;
        Behaviour selectedBehaviour;
        readonly Dictionary<Behaviour, VisualElement> behaviourToElement = new();

        //Behaviour Data Panel
        VisualElement behaviourSequence;
        VisualElement behaviourData;

        //Behavipur
        Toggle toggleBehaviourActive;
        TextField textFieldBehaviourName;

        //Behaviour sequence
        BehaviourEditView behaviourEditView;
        VisualElement nodeInspectorContent;
        Label labelNodeDescription;
        NodeView selectedNodeView;

        //Task Linker
        Label labelTaskSetName;
        VisualElement taskSetContent;
        VisualTreeAsset templateElementLinkedTask;
        VisualElement taskToLinkData;
        ScrollView scrollViewTasksToLink;
        Task selectedTaskInLinker;
        ListView taskListInLinker;
        readonly Dictionary<Task, VisualElement> taskInLinkerToElement = new();

        //Tasks generator panel
        //Sensors
        ToolbarMenu menuSensorSets;
        ToolbarMenu menuSensorType;
        readonly List<int> sensorSetIdsOrdered = new();
        VisualElement sensorsContent;
        VisualElement sensorData;
        VisualElement sensorProperties;
        TextField textFieldSensorName;
        HelpBox helpBoxDetectedObject;
        ListView sensorsListView;
        Sensor selectedSensor;
        readonly Dictionary<Sensor, VisualElement> sensorToElement = new();
        Type selectedSensorType;

        //Indicators
        ToolbarMenu menuIndicatorSets;
        readonly List<int> indicatorSetIdsOrdered = new();
        VisualElement indicatorsContent;
        VisualElement indicatorData;
        VisualElement indicatorProperties;
        TextField textFieldIndicatorName;
        ListView indicatorListView;
        Indicator selectedIndicator;
        readonly Dictionary<Indicator, VisualElement> indicatorToElement = new();

        //Tasks
        ToolbarMenu menuTaskSets;
        readonly List<int> taskSetIdsOrdered = new();
        VisualElement tasksContent;
        VisualElement taskData;
        VisualElement taskProperties;
        TextField textFieldTaskName;
        ListView tasksListView;
        Task selectedTask;
        VisualElement generatedTasks;
        HelpBox generatedTasksList;
        readonly Dictionary<Task, VisualElement> taskToElement = new();
        DropdownField dropdownSensors;
        DropdownField dropdownIndicators;
        EnumField enumFieldComparator;
        FloatField floatFieldValue;


        internal static void OpenWindow()
        {
            var managerWindow = GetWindow<BrainDesignerEditorWindow>();
            managerWindow.titleContent = new GUIContent("Brain Designer");
            managerWindow.minSize = windowMinSize;

        }
        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }
        void OnDestroy()
        {
            this.ClearSubs();

            if (brainDesigner.data == null)
                return;

            EditorUtility.SetDirty(brainDesigner.data);
            AssetDatabase.SaveAssets();
        }

        void CreateGUI()
        {
            //Load main styles
            VisualElement root = rootVisualElement;
            string rootDir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)));

            VisualElement rootUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{rootDir}/UXML/BrainDesigner.uxml").Instantiate();
            root.Add(rootUxml);

            // Store references
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BrainDesigner>() != null)
            {
                brainDesigner = Selection.activeGameObject.GetComponent<BrainDesigner>();

                brainDesigner.Initialize();
                if (brainDesigner.sceneReferences != null)
                    brainDesigner.sceneReferences.Initialize();
            }

            if (brainDesigner == null)
            {
                Close();
                return;
            }

            //Setup main visual elements
            this.SetupUIElements(root, rootDir);

            // Load Asset
            if (brainDesigner.data != null)
            {
                if (!Application.isPlaying)
                    brainDesigner.Load(brainDesigner.data);//
                root.Q<TextField>("TextFieldFileName").value = brainDesigner.data.name;
            }
            
            this.PrepareTaskGeneratorPanel(root);
            this.PrepareBehavoirSetsPanel(root);
            this.PrepareBehaviourDataPanel();
            this.PrepareTaskLinkerPanel(root);
            

            //Prepare controllers for data manipulation.
            root.Q<Button>("ButtonSave").clicked += Save;
            root.Q<Button>("ButtonLoad").clicked += Load;            
        }

        void SetupUIElements(VisualElement root, string rootDir)
        {
            this.textFieldFileName = root.Q<TextField>("TextFieldFileName");
            this.menuBehaviourSets = root.Q<ToolbarMenu>("MenuBehaviourSets");

            this.behaviourSetPanel = root.Q<VisualElement>("BehaviourSetPanel");
            this.behavioursContent = root.Q<VisualElement>("BehavioursContent");
            this.templateElementInSet = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{rootDir}/UXML/TemplateElementInSet.uxml");

            this.behaviourData = root.Q<VisualElement>("BehaviourData");
            this.behaviourSequence = root.Q<VisualElement>("BehaviourSequence");
            this.toggleBehaviourActive = this.behaviourData.Q<Toggle>("ToggleBehaviourActive");
            this.textFieldBehaviourName = this.behaviourData.Q<TextField>("TextFieldBehaviourName");
            this.labelNodeDescription = this.behaviourSequence.Q<Label>("LabelNodeDescription");
            this.nodeInspectorContent = root.Q<VisualElement>("NodeInspectorContent");
            this.behaviourEditView = root.Q<BehaviourEditView>();

            this.menuSensorSets = root.Q<ToolbarMenu>("MenuSensorsSets");
            this.menuSensorType = root.Q<ToolbarMenu>("MenuSensorsTypes");
            this.sensorsContent = root.Q<VisualElement>("SensorsContent");
            this.sensorData = root.Q<VisualElement>("SensorData");
            this.sensorProperties = root.Q<VisualElement>("SensorProperties");
            this.textFieldSensorName = root.Q<TextField>("TextFieldSensorName");

            this.menuIndicatorSets = root.Q<ToolbarMenu>("MenuIndicatorSets");
            this.indicatorsContent = root.Q<VisualElement>("IndicatorsContent");
            this.indicatorData = root.Q<VisualElement>("IndicatorData");
            this.indicatorProperties = root.Q<VisualElement>("IndicatorProperties");
            this.textFieldIndicatorName = root.Q<TextField>("TextFieldIndicatorName");

            this.menuTaskSets = root.Q<ToolbarMenu>("MenuTaskSets");
            this.tasksContent = root.Q<VisualElement>("TasksContent");
            this.taskData = root.Q<VisualElement>("TaskData");
            this.taskProperties = root.Q<VisualElement>("TaskProperties");
            this.textFieldTaskName = root.Q<TextField>("TextFieldTaskName");
            this.dropdownSensors = root.Q<DropdownField>("DropdownSensors");
            this.dropdownIndicators = root.Q<DropdownField>("DropdownIndicators");
            this.generatedTasks = root.Q<VisualElement>("GeneratedTasks");
            this.enumFieldComparator = root.Q<EnumField>("EnumFieldComparator");
            this.floatFieldValue = root.Q<FloatField>("FloatFieldValue");

            this.labelTaskSetName = root.Q<Label>("LabelTaskSetName");
            this.taskSetContent = root.Q<VisualElement>("TaskSetContent");
            this.taskToLinkData = root.Q<VisualElement>("TaskToLinkData");
            this.scrollViewTasksToLink = root.Q<ScrollView>("TasksToLinkList");
            this.templateElementLinkedTask = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{rootDir}/UXML/TemplateElementLinkedTask.uxml");
        }

        void PrepareTaskGeneratorPanel(VisualElement root)
        {
            this.PrepareSensorsPanel(root);
            this.PrepareIndicatorsPanel(root);
            this.PrepareTasksPanel(root);
        }

        void PrepareSensorsPanel(VisualElement root)
        {
            root.Q<Button>("ButtonAddSensorsSet").clicked += AddSensorSet;
            //root.Q<Button>("ButtonRemoveBehaviourSet").clicked += RemoveBehaviourSet;
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddSensor").clicked += AddSensor;
            this.sensorData.style.visibility = Visibility.Hidden;

            SetUtils.LoadSetDropdown(this.menuSensorSets, this.sensorSetIdsOrdered, brainDesigner.GetSensorSets(), this.LoadSensorSet);

            //LoadData
            if (brainDesigner.GetSensorSets().Count == 0)
                this.AddSensorSet();
            else
                this.LoadSensorSet(brainDesigner.selectedSensorSetId);

            //Prepare constant fields.
            this.textFieldSensorName.RegisterValueChangedCallback(change =>
            {
                this.selectedSensor.Name = change.newValue;
                this.sensorsListView.Rebuild();
            });

            LoadSensorTypesDropdown();
        }

        void PrepareIndicatorsPanel(VisualElement root)
        {
            root.Q<Button>("ButtonAddIndicatorSet").clicked += AddIndicatorSet;
            //root.Q<Button>("ButtonRemoveBehaviourSet").clicked += RemoveBehaviourSet;
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddIndicator").clicked += AddIndicator;
            this.indicatorData.style.visibility = Visibility.Hidden;

            SetUtils.LoadSetDropdown(this.menuIndicatorSets, this.indicatorSetIdsOrdered, brainDesigner.GetIndicatorSets(), this.LoadIndicatorSet);

            //LoadData
            if (brainDesigner.GetIndicatorSets().Count == 0)
                this.AddIndicatorSet();
            else
                this.LoadIndicatorSet(brainDesigner.selectedSensorSetId);

            //Prepare constant fields.
            this.textFieldIndicatorName.RegisterValueChangedCallback(change =>
            {
                this.selectedIndicator.Name = change.newValue;
                this.indicatorListView.Rebuild();
            });
        }

        void PrepareTasksPanel(VisualElement root)
        {
            root.Q<Button>("ButtonAddTaskSet").clicked += AddTaskSet;
            //root.Q<Button>("ButtonRemoveBehaviourSet").clicked += RemoveBehaviourSet;
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddTask").clicked += AddTask;
            this.taskData.style.visibility = Visibility.Hidden;

            SetUtils.LoadSetDropdown(this.menuTaskSets, this.taskSetIdsOrdered, brainDesigner.GetTaskSets(), this.LoadTaskSet);

            //LoadData
            if (brainDesigner.GetTaskSets().Count == 0)
                this.AddTaskSet();
            else
                this.LoadTaskSet(brainDesigner.selectedTaskSetId);

            //Prepare constant fields.
            this.textFieldTaskName.RegisterValueChangedCallback(change =>
            {
                this.selectedTask.Name = change.newValue;
                this.tasksListView.Rebuild();
            });

            this.generatedTasksList = new HelpBox("No active tasks", HelpBoxMessageType.Info);
            this.generatedTasks.Add(this.generatedTasksList);
        }

        void PrepareBehavoirSetsPanel(VisualElement root)
        {
            root.Q<Button>("ButtonAddBehaviourSet").clicked += AddBehaviourSet;
            //root.Q<Button>("ButtonRemoveBehaviourSet").clicked += RemoveBehaviourSet;
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddBehaviour").clicked += AddBehaviour;

            SetUtils.LoadSetDropdown(this.menuBehaviourSets, this.behavioureSetIdsOrdered, brainDesigner.GetBehaviourSets(), this.LoadBehaviourSet);

            //LoadData
            if (brainDesigner.GetBehaviourSets().Count == 0)
                this.AddBehaviourSet();
            else
                this.LoadBehaviourSet(brainDesigner.selectedBehaviourSetId);
        }

        void PrepareBehaviourDataPanel()
        {

            VisualElement root = this.behaviourData;
            //Propogate inpector

            this.textFieldBehaviourName.RegisterValueChangedCallback(change =>
            {
                this.selectedBehaviour.Name = change.newValue;
                this.behavioursListView.Rebuild();
            });

            // Add button events
            root.Q<Button>("ButtonAddNode").clicked += AddNode;

            // Initialize          
            this.behaviourEditView.onNodeSelected = OnNodeSelectionChanged;
        }

        void PrepareTaskLinkerPanel(VisualElement root)
        {
            //Init selected task set name.
            //this.taskToLinkData.style.visibility = Visibility.Hidden;

            this.LoadTaskSetFoLinking();
            this.menuTaskSets.RegisterValueChangedCallback(change => this.LoadTaskSetFoLinking());
            brainDesigner.SelectedTaskSet.ListChangedEvent += this.LoadTaskSetFoLinking;

        }

        void ClearSubs()
        {
            VisualElement root = rootVisualElement;
            VisualElement behaviourDataPanel = this.behaviourData;

            root.Q<Button>("ButtonSave").clicked -= Save;
            root.Q<Button>("ButtonLoad").clicked -= Load;

            root.Q<Button>("ButtonAddBehaviourSet").clicked -= AddBehaviourSet;
            //root.Q<Button>("ButtonRemoveBehaviourSet").clicked += RemoveBehaviourSet;
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddBehaviour").clicked -= AddBehaviour;
            behaviourDataPanel.Q<Button>("ButtonAddNode").clicked -= AddNode;

            root.Q<Button>("ButtonAddSensorsSet").clicked -= AddSensorSet;
            root.Q<Button>("ButtonAddSensor").clicked -= AddSensor;

            root.Q<Button>("ButtonAddIndicatorSet").clicked -= AddIndicatorSet;
            root.Q<Button>("ButtonAddIndicator").clicked -= AddIndicator;

            root.Q<Button>("ButtonAddTaskSet").clicked -= AddTaskSet;
            //root.Q<Button>("ButtonRemoveBehaviourSet").clicked += RemoveBehaviourSet;
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddTask").clicked -= AddTask;

            brainDesigner.SelectedTaskSet.ListChangedEvent -= this.LoadTaskSetFoLinking;
        }

        void Save()
        {
            string fileName = this.textFieldFileName.value;

            string savePath = EditorUtility.SaveFilePanel("Save Brain Designer Data", Application.dataPath, fileName, "asset");

            if (savePath.Length != 0)
            {
                if (savePath.StartsWith(Application.dataPath))
                {
                    savePath = "Assets" + savePath.Substring(Application.dataPath.Length);
                    brainDesigner.Save(savePath);
                    Close();
                    OpenWindow();
                }
                else
                    Debug.LogWarning("Invalid save location. Please choose a location within the project's Assets directory.");
            }
        }

        void Load()
        {
            string loadPath = EditorUtility.OpenFilePanel(
                "Load Brain Designer Data", Application.dataPath, "asset");

            if (loadPath.Length != 0)
            {
                if (loadPath.StartsWith(Application.dataPath))
                {
                    if (brainDesigner.data == null)
                    {
                        if (!EditorUtility.DisplayDialog("Confirmation",
                                "Are you sure you want to *OVERRIDE* everything?",
                                "Yes", "No"))
                            return;
                    }

                    loadPath = "Assets" + loadPath.Substring(Application.dataPath.Length);
                    if (!brainDesigner.TryLoad(loadPath))
                        return;

                    Close();
                    OpenWindow();
                }
                else
                    Debug.LogWarning("Invalid load location. Please choose a location within the project's Assets directory.");
            }
        }

        #region BEHAVIOUR SET

        void LoadBehaviourSet(int id)
        {
            brainDesigner.selectedBehaviourSetId = id; ;
            this.menuBehaviourSets.text = brainDesigner.GetBehaviourSets()[id].Name;

            this.selectedBehaviour = null;
            this.behaviourData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList<Behaviour>(new SetUtils.SetData<Behaviour>(this.behavioursContent, this.behavioursListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.SelectedBehaviourSet, this.behaviourToElement, this.selectedBehaviour, SelectionType.Single), this.LoadBehaviour, this.behaviourData);

            this.behavioursListView = newSetData.listView;
            if (brainDesigner.SelectedBehaviourSet.list.Count > 0)
            {
                this.behavioursListView.SetSelection(0);
            }
        }

        void AddBehaviourSet()
        {
            int currentId = brainDesigner.GetBehaviourSets().Count;
            string newBehaviourSetName = $"Behaviour Set {currentId + 1}";
            brainDesigner.AddBehaviourSet(currentId, new BehaviourSet
            {
                Name = newBehaviourSetName
            });

            this.menuBehaviourSets.menu.AppendAction(newBehaviourSetName, action => LoadBehaviourSet(currentId));

            this.behavioureSetIdsOrdered.Add(currentId);

            LoadBehaviourSet(currentId);
        }

        /* private void RemoveStateSet()
         {
             if (_menuStateSets.menu.MenuItems().Count <= 1)
                 return;

             if (!EditorUtility.DisplayDialog("Confirmation",
                     "Are you sure you want to *DELETE* this state set?",
                     "Yes", "No"))
                 return;

             _menuStateSets.menu.RemoveItemAt(_stateSetIdsOrdered.IndexOf(_utilityDesigner.selectedStateSetId));
             _utilityDesigner.RemoveStateSet(_utilityDesigner.selectedStateSetId);
             _stateSetIdsOrdered.Remove(_utilityDesigner.selectedStateSetId);

             LoadStateSet(_stateSetIdsOrdered.Last());
         }*/

        /* private void OpenStateSetRenameMenu()
         {
             if (_stateSetTab.childCount == 2)
             {
                 TextField textFieldRenameStateSet = new TextField
                 {
                     label = "New name",
                     value = _utilityDesigner.SelectedStateSet.designation,
                     maxLength = 16
                 };
                 textFieldRenameStateSet.RegisterValueChangedCallback(evt =>
                 {
                     string newDesignation = Utils.VerifyItemName("State Set ",
                         evt.newValue,
                         _utilityDesigner.GetStateSets().Values,
                         state => state.designation,
                         _utilityDesigner.SelectedStateSet?.designation);
                     if (_utilityDesigner.SelectedStateSet != null)
                         _utilityDesigner.SelectedStateSet.designation = newDesignation;
                     _menuStateSets.text = newDesignation;
                     LoadStateSetDropdown();
                 });

                 _stateSetTab.Insert(1, textFieldRenameStateSet);
             }
             else
                 _stateSetTab.RemoveAt(1);
         }*/
        #endregion

        #region BEHAVIOUR
        void LoadBehaviour(IEnumerable<object> selectedItems)
        {
            this.selectedBehaviour = (Behaviour)selectedItems.First();

            this.behaviourData.style.visibility = Visibility.Visible;
            this.LoadBehaviourProperties();
            this.LoadBehaviourSequence();
        }

        void AddBehaviour()
        {
            var currentSetBehaviours = brainDesigner.SelectedBehaviourSet.list;
            currentSetBehaviours.Add(new Behaviour
            {
                active = true,
                Name = $"Behaviour {currentSetBehaviours.Count + 1}"
            });

            this.behavioursListView.style.height = currentSetBehaviours.Count * elementInSetHeight;
            this.behavioursListView.SetSelection(currentSetBehaviours.Count - 1);
            this.behavioursListView.Rebuild();
        }

        void LoadBehaviourSequence()
        {

            this.behaviourEditView.PopulateView(this.selectedBehaviour, brainDesigner, this);
        }

        void LoadBehaviourProperties()
        {
            this.toggleBehaviourActive.value = this.selectedBehaviour.active;
            this.textFieldBehaviourName.value = this.selectedBehaviour.Name;
        }

        //---NODE INSPECTOR

        void AddNode() => this.behaviourEditView.CreateContextualMenu(new Vector2(0, 0));

        void OnNodeSelectionChanged(NodeView nodeView)
        {
            this.nodeInspectorContent.Clear();
            this.labelNodeDescription.text = "";
            this.selectedNodeView = nodeView;

            if (nodeView == null)
                return;

            this.DrawFields(nodeView.node, this.nodeInspectorContent);
        }


        #endregion

        #region SENSORS


        void LoadSensorSet(int id)
        {
            brainDesigner.selectedSensorSetId = id;
            this.menuSensorSets.text = brainDesigner.GetSensorSets()[id].Name;

            this.selectedSensor = null;
            this.sensorData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList(new SetUtils.SetData<Sensor>(this.sensorsContent, this.sensorsListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.SelectedSensorSet, this.sensorToElement, this.selectedSensor, SelectionType.Single), this.LoadSensor, this.sensorData);

            this.sensorsListView = newSetData.listView;
            if (brainDesigner.SelectedSensorSet.list.Count > 0)
            {
                this.sensorsListView.SetSelection(0);
            }
        }

        void AddSensorSet()
        {
            int currentId = brainDesigner.GetSensorSets().Count;
            string newSensorSetName = $"Sensor Set {currentId + 1}";
            brainDesigner.AddSensorSet(currentId, new SensorSet
            {
                Name = newSensorSetName
            });
            //
            this.menuSensorSets.menu.AppendAction(newSensorSetName, action => LoadSensorSet(currentId));

            this.sensorSetIdsOrdered.Add(currentId);

            LoadSensorSet(currentId);
        }

        void LoadSensorTypesDropdown()
        {
            this.menuSensorType.menu.ClearItems();

            var types = TypeCache.GetTypesDerivedFrom<Sensor>();
            foreach (var type in types)
            {
                var name = $"{Utils.AddSpacesBeforeUppercase(type.Name)}";
                this.menuSensorType.menu.AppendAction(name, action => { this.selectedSensorType = type; });
                if (this.selectedSensorType == null)
                {
                    this.menuSensorType.text = name;
                    this.selectedSensorType = type;
                }
            }
        }
        void AddSensor()
        {
            if (Activator.CreateInstance(this.selectedSensorType) is not Sensor sensor)
            {
                var helpBox = new HelpBox("Invalid sensor was chosen", HelpBoxMessageType.Warning);
                this.sensorsContent.Add(helpBox);
            }
            else
            {
                sensor.Name = Utils.AddSpacesBeforeUppercase(sensor.GetType().Name);
                var currentSensorSet = brainDesigner.SelectedSensorSet.list;
                currentSensorSet.Add(sensor);
                this.sensorsListView.style.height = currentSensorSet.Count * elementInSetHeight;
                this.sensorsListView.ClearSelection();
                this.sensorsListView.SetSelection(currentSensorSet.Count - 1);
                this.sensorsListView.Rebuild();
            }
        
        }

        void LoadSensor(IEnumerable<object> selectedItems)
        {
            if (!selectedItems.Any()) return;
            this.selectedSensor = (Sensor)selectedItems.First();
            this.LoadSensorProperties(this.selectedSensor);

            //brainDesigner.lastSelectedBehaviourIndex = brainDesigner.SelectedBehaviourSet.list.IndexOf(selectedBehaviour);
        }

        void LoadSensorProperties(Sensor sensor)
        {
            if (sensor == null)
                return;

            this.sensorProperties.Clear();
            this.sensorData.style.visibility = Visibility.Visible;

            this.textFieldSensorName.value = this.selectedSensor.Name;
            this.DrawFields(sensor, this.sensorProperties);
            if (this.helpBoxDetectedObject != null) return;
            
            this.helpBoxDetectedObject = new HelpBox("Not active", HelpBoxMessageType.Warning);
            this.sensorData.Add(this.helpBoxDetectedObject);
        }

        #endregion

        #region Indicators


        void LoadIndicatorSet(int id)
        {
            brainDesigner.selectedIndicatorSetId = id;
            this.menuIndicatorSets.text = brainDesigner.GetIndicatorSets()[id].Name;

            this.selectedIndicator = null;
            this.indicatorData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList(new SetUtils.SetData<Indicator>(this.indicatorsContent, this.indicatorListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.SelectedIndicatorSet, this.indicatorToElement, this.selectedIndicator, SelectionType.Single), this.LoadIndicator, this.indicatorData);

            this.indicatorListView = newSetData.listView;
            if (brainDesigner.SelectedIndicatorSet.list.Count > 0)
            {
                this.indicatorListView.SetSelection(0);
            }
        }

        void AddIndicatorSet()
        {
            int currentId = brainDesigner.GetIndicatorSets().Count;
            string newIndicatorSetName = $"Indicator Set {currentId + 1}";
            brainDesigner.AddIndicatorSet(currentId, new IndicatorSet
            {
                Name = newIndicatorSetName
            });

            this.menuIndicatorSets.menu.AppendAction(newIndicatorSetName, action => this.LoadIndicatorSet(currentId));

            this.indicatorSetIdsOrdered.Add(currentId);

            this.LoadIndicatorSet(currentId);
        }

   
        void AddIndicator()
        {
            var currentIndicatorSet = brainDesigner.SelectedIndicatorSet.list;
            currentIndicatorSet.Add(new Indicator
            {
                Name = $"Indicator {currentIndicatorSet.Count + 1}"
            });
           
                this.indicatorListView.style.height = currentIndicatorSet.Count * elementInSetHeight;
                this.indicatorListView.SetSelection(currentIndicatorSet.Count - 1);
                this.indicatorListView.Rebuild();
        }

        void LoadIndicator(IEnumerable<object> selectedItems)
        {
            this.selectedIndicator = (Indicator)selectedItems.First();
            this.LoadIndicatorProperties(this.selectedIndicator);

            //brainDesigner.lastSelectedBehaviourIndex = brainDesigner.SelectedBehaviourSet.list.IndexOf(selectedBehaviour);
        }

        void LoadIndicatorProperties(Indicator indicator)
        {
            if (indicator == null)
                return;

            this.indicatorProperties.Clear();
            this.indicatorData.style.visibility = Visibility.Visible;

            this.textFieldIndicatorName.value = this.selectedIndicator.Name;
            this.DrawFields(indicator, this.indicatorProperties);
        }

        #endregion

        #region TaskSet

        void LoadTaskSet(int id)
        {
            brainDesigner.selectedTaskSetId = id;
            this.menuTaskSets.text = brainDesigner.GetTaskSets()[id].Name;

            this.selectedTask = null;
            this.taskData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList(new SetUtils.SetData<Task>(this.tasksContent, this.tasksListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.SelectedTaskSet, this.taskToElement, this.selectedTask, SelectionType.Single), this.LoadTask, this.taskData);

            this.tasksListView = newSetData.listView;
            if (brainDesigner.SelectedTaskSet.list.Count > 0)
            {
                this.tasksListView.SetSelection(0);
            }
            
        }

        void AddTaskSet()
        {
            int currentId = brainDesigner.GetTaskSets().Count;
            string newTaskSetName = $"Task Set {currentId + 1}";
            brainDesigner.AddTaskSet(currentId, new TaskSet
            {
                Name = newTaskSetName
            });

            this.menuTaskSets.menu.AppendAction(newTaskSetName, action => this.LoadTaskSet(currentId));

            this.taskSetIdsOrdered.Add(currentId);

            this.LoadTaskSet(currentId);
        }


        void AddTask()
        {
            var currentTaskSet = brainDesigner.SelectedTaskSet.list;
            brainDesigner.SelectedTaskSet.AddSetElement(new Task
            {
                Name = $"Indicator {currentTaskSet.Count + 1}"
            });

            this.tasksListView.style.height = currentTaskSet.Count * elementInSetHeight;
            this.tasksListView.SetSelection(currentTaskSet.Count - 1);
            this.tasksListView.Rebuild();
        }

        void LoadTask(IEnumerable<object> selectedItems)
        {
            this.selectedTask = (Task)selectedItems.First();
            this.taskData.style.visibility = Visibility.Visible;
            this.textFieldTaskName.value = this.selectedTask.Name;
            this.LoadTaskManualFields(this.selectedTask);
            this.LoadTaskProperties(this.selectedTask);

            //brainDesigner.lastSelectedBehaviourIndex = brainDesigner.SelectedBehaviourSet.list.IndexOf(selectedBehaviour);
        }
        void LoadTaskManualFields(Task task)
        {
            this.InitializeSensorsDropdown();
            this.InitializeIndicatorsDropdown();
        }

        void LoadTaskProperties(Task task)
        {
            if (task == null)
                return;

            this.taskProperties.Clear();

            //this.textFieldTaskName.value = this.selectedTask.Name;
            this.DrawFields(task, this.taskProperties);
        }

        void InitializeSensorsDropdown()
        {
            this.dropdownSensors.choices = brainDesigner.SelectedSensorSet.list.Select(item => item.Name).ToList();
            this.dropdownSensors.RegisterValueChangedCallback(evt => this.selectedTask.SensorToCheckName = evt.newValue);
            this.dropdownSensors.SetValueWithoutNotify(this.dropdownSensors.choices.Contains(this.selectedTask.SensorToCheckName) ? $"{this.selectedTask.SensorToCheckName}" : "");
        }

        void InitializeIndicatorsDropdown()
        {
            this.dropdownIndicators.choices = brainDesigner.SelectedIndicatorSet.list.Select(item => item.Name).ToList();
            this.dropdownIndicators.RegisterValueChangedCallback(evt => this.selectedTask.IndicatorToCheckName = evt.newValue);
            this.dropdownIndicators.SetValueWithoutNotify(this.dropdownIndicators.choices.Contains(this.selectedTask.IndicatorToCheckName) ? $"{this.selectedTask.IndicatorToCheckName}" : "");
            
            this.enumFieldComparator.RegisterValueChangedCallback(evt => this.selectedTask.Comparator = (Comparator)evt.newValue);
            this.enumFieldComparator.SetValueWithoutNotify(this.selectedTask.Comparator);
            
            this.floatFieldValue.RegisterValueChangedCallback(evt => this.selectedTask.CompareValue = evt.newValue);
            this.floatFieldValue.SetValueWithoutNotify(this.selectedTask.CompareValue);
            
        }

        #endregion

        #region TaskLinker

        void LoadTaskSetFoLinking()
        {
            this.labelTaskSetName.text = brainDesigner.SelectedTaskSet.Name;
            this.scrollViewTasksToLink.Clear();
            this.selectedTaskInLinker = null;

            foreach (var task in brainDesigner.SelectedTaskSet.list)
                this.AddElement(task);

            this.scrollViewTasksToLink.contentContainer.style.flexDirection = FlexDirection.Row;
        }

        void AddElement(Task task)
        {
            var newElement = this.templateElementLinkedTask.CloneTree();
            newElement.style.width = elementInTaskLinked;
            newElement.style.flexShrink = 0;

            newElement.Q<Label>("LabelTitle").text = task.Name;
            this.LoadBehaviourList(task, newElement);
            this.LoadBehaviourDropdown(task, newElement);

            this.scrollViewTasksToLink.contentContainer.Add(newElement);
        }

        void LoadBehaviourList(Task task, TemplateContainer element)
        {            
            var linkedBehavioursContant = element.Q<VisualElement>("LinkedTaskBehaviours");
            
        }

        void LoadBehaviourDropdown(Task task, TemplateContainer element)
        {
            var dropdownBehaviours = element.Q<DropdownField>("DropdownBehaviours");
            var buttonLinkBehaviour = element.Q<Button>("ButtonLinkBehaviour");
            var currenBehaviourList = brainDesigner.SelectedBehaviourSet.list;

            dropdownBehaviours.choices = currenBehaviourList.Select(item => item.Name).ToList();
            dropdownBehaviours.SetValueWithoutNotify(currenBehaviourList.Count > 0 ? currenBehaviourList.FirstOrDefault().Name : "-");
        }

        #endregion

        internal void DrawFields(object obj, VisualElement parentContainer)
        {
            if (obj == null)
            {
                var helpBox = new HelpBox("Action is not assigned.", HelpBoxMessageType.Warning);
                parentContainer.Add(helpBox);
                return;
            }

            if (obj is Node node)
                node.InitializedDropdowns(parentContainer);

            Type objType = obj.GetType();
            FieldInfo[] fields = objType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if ((obj is Node && field.Name == "sceneReferences") || (field.IsPrivate && !field.IsAssembly) || obj is Sequence)
                    continue;

                object fieldValue = field.GetValue(obj);
                Type fieldType = field.FieldType;
                string displayName = Utils.VariableNameToReadable(field.Name);

                if (fieldType == typeof(int))
                {
                    var fieldElement = new IntegerField(displayName);
                    fieldElement.SetValueWithoutNotify((int)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(float))
                {
                    var fieldElement = new FloatField(displayName);
                    fieldElement.SetValueWithoutNotify((float)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(double))
                {
                    var fieldElement = new DoubleField(displayName);
                    fieldElement.SetValueWithoutNotify((double)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(string))
                {
                    bool isDescriptionFieldInBaseNode = obj is Node && field.Name == "notes";
                    TextField fieldElement;

                    if (isDescriptionFieldInBaseNode)
                    {
                        if (parentContainer.childCount != 0)
                        {
                            var spacer = new VisualElement
                            {
                                style =
                                {
                                    height = 15
                                }
                            };
                            this.nodeInspectorContent.Add(spacer);
                        }

                        fieldElement = new TextField(displayName)
                        {
                            multiline = true,
                            style =
                            {
                                height = 70,
                                whiteSpace = WhiteSpace.Normal
                            }
                        };
                    }
                    else
                        fieldElement = new TextField(displayName);

                    fieldElement.SetValueWithoutNotify((string)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);

                }
                else if (fieldType == typeof(bool))
                {
                    var fieldElement = new Toggle(displayName);
                    fieldElement.SetValueWithoutNotify((bool)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType.IsEnum)
                {
                    var fieldElement = new EnumField(displayName, (Enum)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Vector2))
                {
                    var fieldElement = new Vector2Field(displayName);
                    fieldElement.SetValueWithoutNotify((Vector2)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Vector3))
                {
                    var fieldElement = new Vector3Field(displayName);
                    fieldElement.SetValueWithoutNotify((Vector3)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Vector4))
                {
                    var fieldElement = new Vector4Field(displayName);
                    fieldElement.SetValueWithoutNotify((Vector4)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Vector2Int))
                {
                    var fieldElement = new Vector2IntField(displayName);
                    fieldElement.SetValueWithoutNotify((Vector2Int)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Vector3Int))
                {
                    var fieldElement = new Vector3IntField(displayName);
                    fieldElement.SetValueWithoutNotify((Vector3Int)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Color))
                {
                    var fieldElement = new ColorField(displayName);
                    fieldElement.SetValueWithoutNotify((Color)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Rect))
                {
                    var fieldElement = new RectField(displayName);
                    fieldElement.SetValueWithoutNotify((Rect)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Bounds))
                {
                    var fieldElement = new BoundsField(displayName);
                    fieldElement.SetValueWithoutNotify((Bounds)fieldValue);
                    fieldElement.RegisterValueChangedCallback(evt => field.SetValue(obj, evt.newValue));
                    parentContainer.Add(fieldElement);
                }
                else if (fieldType == typeof(Quaternion))
                {
                    var fieldElement = new Vector3Field($"{displayName} (Euler Angles)");
                    Quaternion quaternionValue = (Quaternion)fieldValue;
                    Vector3 eulerAngles = quaternionValue.eulerAngles;
                    fieldElement.SetValueWithoutNotify(eulerAngles);
                    fieldElement.RegisterValueChangedCallback(evt =>
                    {
                        Quaternion newValue = Quaternion.Euler(evt.newValue);
                        field.SetValue(obj, newValue);
                    });
                    parentContainer.Add(fieldElement);
                }
                else if (!fieldType.IsPrimitive && !fieldType.IsEnum &&
                         !typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
                {
                    var foldout = new Foldout { text = $"{displayName} ({fieldType.Name})" };
                    var container = new VisualElement();
                    foldout.Add(container);
                    foldout.RegisterValueChangedCallback(evt =>
                    {
                        if (evt.newValue)
                            this.DrawFields(fieldValue, parentContainer);
                        else
                            container.Clear();
                    });
                    parentContainer.Add(foldout);
                }
                else
                {
                    var unsupportedLabel = new Label($"{displayName}: Type '{fieldType.Name}' is not supported");
                    parentContainer.Add(unsupportedLabel);
                }
            }
        }

        void OnPlayModeChanged(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.EnteredEditMode)
                return;

            Close();
            if (Selection.activeGameObject != null &&
                Selection.activeGameObject.GetComponent<BrainDesigner>() != null)
                OpenWindow();
        }

        void OnInspectorUpdate()
        {
            if (!Application.isPlaying)
                return;

            if (this.behaviourData.visible)
                this.behaviourEditView.UpdateNodeStates();

            if (this.sensorData.visible && this.helpBoxDetectedObject != null)
                this.selectedSensor.UpdateSensorStateInfo(this.helpBoxDetectedObject);

            if (this.taskData.visible && this.generatedTasks != null)
                brainDesigner.UpdateGeneratedTasksInfo(this.generatedTasksList);
        }
    }
}
#endif