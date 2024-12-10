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
        VisualElement behavioursContent;
        ListView behavioursListView;
        Behaviour selectedBehaviour;
        readonly Dictionary<Behaviour, VisualElement> behaviourToElement = new();

        //Behaviour Data Panel
        VisualElement behaviourSequence;
        VisualElement behaviourData;

        //Behavipur
        Toggle toggleBehaviourActive;
        Toggle toggleBehaviourCritical;
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

        ListView taskListInLinker;
        readonly Dictionary<Task, VisualElement> taskInLinkerToElement = new();

        //Tasks generator panel
        //Sensors
        ToolbarMenu menuSensorType;
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
        VisualElement indicatorsContent;
        VisualElement indicatorData;
        VisualElement indicatorProperties;
        TextField textFieldIndicatorName;
        ListView indicatorListView;
        Indicator selectedIndicator;
        readonly Dictionary<Indicator, VisualElement> indicatorToElement = new();

        //Tasks
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
        EnumField enumFieldComparatorFirst;
        FloatField floatFieldValueFirst;
        VisualElement secondConditionContainer;
        Toggle toggleNeedSecondCondition;
        EnumField enumFieldComparatorSecond;
        FloatField floatFieldValueSecond;


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
            this.behavioursContent = root.Q<VisualElement>("BehavioursContent");
            this.templateElementInSet = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{rootDir}/UXML/TemplateElementInSet.uxml");

            this.behaviourData = root.Q<VisualElement>("BehaviourData");
            this.behaviourSequence = root.Q<VisualElement>("BehaviourSequence");
            this.toggleBehaviourActive = this.behaviourData.Q<Toggle>("ToggleBehaviourActive");
            this.toggleBehaviourCritical = this.behaviourData.Q<Toggle>("ToggleBehaviourCritical");
            this.textFieldBehaviourName = this.behaviourData.Q<TextField>("TextFieldBehaviourName");
            this.labelNodeDescription = this.behaviourSequence.Q<Label>("LabelNodeDescription");
            this.nodeInspectorContent = root.Q<VisualElement>("NodeInspectorContent");
            this.behaviourEditView = root.Q<BehaviourEditView>();

            this.menuSensorType = root.Q<ToolbarMenu>("MenuSensorsTypes");
            this.sensorsContent = root.Q<VisualElement>("SensorsContent");
            this.sensorData = root.Q<VisualElement>("SensorData");
            this.sensorProperties = root.Q<VisualElement>("SensorProperties");
            this.textFieldSensorName = root.Q<TextField>("TextFieldSensorName");

            this.indicatorsContent = root.Q<VisualElement>("IndicatorsContent");
            this.indicatorData = root.Q<VisualElement>("IndicatorData");
            this.indicatorProperties = root.Q<VisualElement>("IndicatorProperties");
            this.textFieldIndicatorName = root.Q<TextField>("TextFieldIndicatorName");

            this.tasksContent = root.Q<VisualElement>("TasksContent");
            this.taskData = root.Q<VisualElement>("TaskData");
            this.taskProperties = root.Q<VisualElement>("TaskProperties");
            this.textFieldTaskName = root.Q<TextField>("TextFieldTaskName");
            this.dropdownSensors = root.Q<DropdownField>("DropdownSensors");
            this.dropdownIndicators = root.Q<DropdownField>("DropdownIndicators");
            this.generatedTasks = root.Q<VisualElement>("GeneratedTasks");
            this.enumFieldComparatorFirst = root.Q<EnumField>("EnumFieldComparatorFirst");
            this.floatFieldValueFirst = root.Q<FloatField>("FloatFieldValueFirst");
            this.toggleNeedSecondCondition = root.Q <Toggle>("ToggleNeedSecondCondition");
            this.secondConditionContainer = root.Q<VisualElement>("SecondConditionContainer");
            this.enumFieldComparatorSecond = root.Q<EnumField>("EnumFieldComparatorSecond");
            this.floatFieldValueSecond = root.Q<FloatField>("FloatFieldValueSecond");

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
            
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddSensor").clicked += AddSensor;
            this.sensorData.style.visibility = Visibility.Hidden;

            //LoadData
            if (brainDesigner.SensorSet == null)
                this.AddSensorSet();
            else
                this.LoadSensorSet();

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
           
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddIndicator").clicked += AddIndicator;
            this.indicatorData.style.visibility = Visibility.Hidden;

            //LoadData
            if (brainDesigner.IndicatorSet == null)
                this.AddIndicatorSet();
            else
                this.LoadIndicatorSet();

            //Prepare constant fields.
            this.textFieldIndicatorName.RegisterValueChangedCallback(change =>
            {
               this.selectedIndicator.Name = change.newValue;
                this.indicatorListView.Rebuild();
            });
        }

        void PrepareTasksPanel(VisualElement root)
        {
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddTask").clicked += AddTask;
            this.taskData.style.visibility = Visibility.Hidden;

            //LoadData
            if (brainDesigner.TaskSet == null)
                this.AddTaskSet();
            else
                this.LoadTaskSet();

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
            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddBehaviour").clicked += AddBehaviour;

            //LoadData
            if (brainDesigner.BehaviourSet == null)
                this.AddBehaviourSet();
            else
                this.LoadBehaviourSet();
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
            // Register value changed callbacks
            this.toggleBehaviourCritical.RegisterValueChangedCallback(evt =>
            {
                this.selectedBehaviour.critical = evt.newValue;
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
            brainDesigner.TaskSet.ListChangedEvent += this.LoadTaskSetFoLinking;

        }

        void ClearSubs()
        {
            VisualElement root = rootVisualElement;
            VisualElement behaviourDataPanel = this.behaviourData;

            root.Q<Button>("ButtonSave").clicked -= Save;
            root.Q<Button>("ButtonLoad").clicked -= Load;

            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddBehaviour").clicked -= AddBehaviour;
            behaviourDataPanel.Q<Button>("ButtonAddNode").clicked -= AddNode;

            root.Q<Button>("ButtonAddSensor").clicked -= AddSensor;

            root.Q<Button>("ButtonAddIndicator").clicked -= AddIndicator;

            //root.Q<Button>("ButtonRenameBehaviourSet").clicked += OpenBehaviourSetRenameMenu;
            root.Q<Button>("ButtonAddTask").clicked -= AddTask;

            brainDesigner.TaskSet.ListChangedEvent -= this.LoadTaskSetFoLinking;
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

        void LoadBehaviourSet()
        {
            this.selectedBehaviour = null;
            this.behaviourData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList<Behaviour>(new SetUtils.SetData<Behaviour>(this.behavioursContent, this.behavioursListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.BehaviourSet, this.behaviourToElement, this.selectedBehaviour, SelectionType.Single), this.LoadBehaviour, this.behaviourData);

            this.behavioursListView = newSetData.listView;
            if (brainDesigner.BehaviourSet.list.Count > 0)
            {
                this.behavioursListView.SetSelection(0);
            }
        }

        void AddBehaviourSet()
        {
            string newBehaviourSetName = $"Behaviour Set";
            brainDesigner.BehaviourSet = new BehaviourSet { Name = newBehaviourSetName };

            LoadBehaviourSet();
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
            var currentSetBehaviours = brainDesigner.BehaviourSet.list;
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
            this.toggleBehaviourCritical.value = this.selectedBehaviour.critical;
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

        void LoadSensorSet()
        {
            this.selectedSensor = null;
            this.sensorData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList(new SetUtils.SetData<Sensor>(this.sensorsContent, this.sensorsListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.SensorSet, this.sensorToElement, this.selectedSensor, SelectionType.Single), this.LoadSensor, this.sensorData);

            this.sensorsListView = newSetData.listView;
            if (brainDesigner.SensorSet.list.Count > 0)
            {
                this.sensorsListView.SetSelection(0);
            }
        }

        void AddSensorSet()
        {
            string newSensorSetName = $"Sensor Set";
            brainDesigner.SensorSet = new SensorSet {Name = newSensorSetName};
            LoadSensorSet();
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
                var currentSensorSet = brainDesigner.SensorSet.list;
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


        void LoadIndicatorSet()
        {
            this.selectedIndicator = null;
            this.indicatorData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList(new SetUtils.SetData<Indicator>(this.indicatorsContent, this.indicatorListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.IndicatorSet, this.indicatorToElement, this.selectedIndicator, SelectionType.Single), this.LoadIndicator, this.indicatorData);

            this.indicatorListView = newSetData.listView;
            if (brainDesigner.IndicatorSet.list.Count > 0)
            {
                this.indicatorListView.SetSelection(0);
            }
        }

        void AddIndicatorSet()
        {
            string newIndicatorSetName = $"Indicator Set";
            brainDesigner.IndicatorSet = new IndicatorSet{ Name = newIndicatorSetName };
            this.LoadIndicatorSet();
        }

   
        void AddIndicator()
        {
            var currentIndicatorSet = brainDesigner.IndicatorSet.list;
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

        void LoadTaskSet()
        {
            this.selectedTask = null;
            this.taskData.style.visibility = Visibility.Hidden;

            var newSetData = SetUtils.CreateSetList(new SetUtils.SetData<Task>(this.tasksContent, this.tasksListView, this.templateElementInSet, this.elementInSetHeight, brainDesigner.TaskSet, this.taskToElement, this.selectedTask, SelectionType.Single), this.LoadTask, this.taskData);

            this.tasksListView = newSetData.listView;
            if (brainDesigner.TaskSet.list.Count > 0)
            {
                this.tasksListView.SetSelection(0);
            }            
        }

        void AddTaskSet()
        {
            string newTaskSetName = $"Task Set";
            brainDesigner.TaskSet = new TaskSet{ Name = newTaskSetName };
            this.LoadTaskSet();
        }

        void AddTask()
        {
            var currentTaskSet = brainDesigner.TaskSet.list;
            brainDesigner.TaskSet.AddSetElement(new Task
            {
                Name = $"Task {currentTaskSet.Count + 1}"
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
            this.dropdownSensors.choices = brainDesigner.SensorSet.list.Select(item => item.Name).ToList();
            this.dropdownSensors.RegisterValueChangedCallback(evt => this.selectedTask.SensorToCheckName = evt.newValue);
            this.dropdownSensors.SetValueWithoutNotify(this.dropdownSensors.choices.Contains(this.selectedTask.SensorToCheckName) ? $"{this.selectedTask.SensorToCheckName}" : "");
        }

        void InitializeIndicatorsDropdown()
        {
            this.dropdownIndicators.choices = brainDesigner.IndicatorSet.list.Select(item => item.Name).ToList();
            this.dropdownIndicators.RegisterValueChangedCallback(evt => this.selectedTask.IndicatorToCheckName = evt.newValue);
            this.dropdownIndicators.SetValueWithoutNotify(this.dropdownIndicators.choices.Contains(this.selectedTask.IndicatorToCheckName) ? $"{this.selectedTask.IndicatorToCheckName}" : "");

            this.toggleNeedSecondCondition.RegisterValueChangedCallback(evt =>
            {
                this.selectedTask.NeedSecondCondition = evt.newValue;
                this.SetSecondConditionContainer(this.selectedTask.NeedSecondCondition);
                this.tasksListView.Rebuild();
            });
            this.toggleNeedSecondCondition.value = this.selectedTask.NeedSecondCondition;
         
            this.enumFieldComparatorFirst.RegisterValueChangedCallback(evt => this.selectedTask.ComparatorFirst = (Comparator)evt.newValue);
            this.enumFieldComparatorFirst.SetValueWithoutNotify(this.selectedTask.ComparatorFirst);
            this.floatFieldValueFirst.RegisterValueChangedCallback(evt => this.selectedTask.CompareValueFirst = evt.newValue);
            this.floatFieldValueFirst.SetValueWithoutNotify(this.selectedTask.CompareValueFirst);

            this.SetSecondConditionContainer(this.selectedTask.NeedSecondCondition);

            this.enumFieldComparatorSecond.RegisterValueChangedCallback(evt => this.selectedTask.ComparatorSecond = (Comparator)evt.newValue);
            this.enumFieldComparatorSecond.SetValueWithoutNotify(this.selectedTask.ComparatorSecond);
            this.floatFieldValueSecond.RegisterValueChangedCallback(evt => this.selectedTask.CompareValueSecond = evt.newValue);
            this.floatFieldValueSecond.SetValueWithoutNotify(this.selectedTask.CompareValueSecond);            
        }

        void SetSecondConditionContainer(bool show)
        {
            this.secondConditionContainer.style.visibility = show ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion

        #region TaskLinker

        void LoadTaskSetFoLinking()
        {
            this.labelTaskSetName.text = brainDesigner.TaskSet.Name;
            this.scrollViewTasksToLink.Clear();

            foreach (var task in brainDesigner.TaskSet.list)
                this.AddElement(task);

            this.scrollViewTasksToLink.contentContainer.style.flexDirection = FlexDirection.Row;
        }

        void AddElement(Task task)
        {
            var newElement = this.templateElementLinkedTask.CloneTree();
            newElement.style.width = elementInTaskLinked;
            newElement.style.flexShrink = 0;

            newElement.Q<Label>("LabelTitle").text = task.Name;
            ListView elementBehaviourList = null;

            this.LoadBehaviourList(task, newElement, ref elementBehaviourList);
            this.LoadBehaviourDropdown(task, newElement, elementBehaviourList);

            this.scrollViewTasksToLink.contentContainer.Add(newElement);
        }

        void LoadBehaviourList(Task task, TemplateContainer element, ref ListView listView)
        {
            var linkedBehavioursContent = element.Q<VisualElement>("LinkedTaskBehaviours");
            listView = CreateBehaviourList(linkedBehavioursContent, this.templateElementInSet, task, task.LinkedBehaviours, listView, 20);
            
        }

        void LoadBehaviourDropdown(Task task, TemplateContainer element, ListView listView)
        {
            var dropdownBehaviours = element.Q<DropdownField>("DropdownBehaviours");
            var buttonLinkBehaviour = element.Q<Button>("ButtonLinkBehaviour");
            var currenBehaviourList = brainDesigner.BehaviourSet.list;

            dropdownBehaviours.choices = currenBehaviourList.Select(item => item.Name).ToList();
            dropdownBehaviours.SetValueWithoutNotify(currenBehaviourList.Count > 0 ? currenBehaviourList.FirstOrDefault().Name : "-");

            buttonLinkBehaviour.clickable.clicked += () =>
            {
                if (brainDesigner.BehaviourSet.TryGetElementByName(dropdownBehaviours.value, out var behaviourToAdd))
                {
                    task.LinkUniqueBehaviour(behaviourToAdd);
                    listView.Rebuild();
                }
            };
        }

        internal static ListView CreateBehaviourList(VisualElement parentContainer, VisualTreeAsset templateElement, Task task, List<Behaviour> linkedBehaviours, ListView listView, float elementHeight)
        {
            System.Action currentRemoveButtonAction = null;
            parentContainer.Clear();

            VisualElement CreateElement() => templateElement.Instantiate();

            static void OnButtonRemoveElementClick(Task task, List<Behaviour> linkedBehaviours, ListView listView, int i, Behaviour behaviourReference)
            {
                task.UnlinqBehaviorAt(i, linkedBehaviours[i]);
                listView.Rebuild();
            }

            //additional method Û

            void BindElement(VisualElement e, int i)
            {
                var elementReference = linkedBehaviours[i];

                if (elementReference is not Named)
                {
                    Utils.ShowHelpBoxInEditor(parentContainer, "Type of element is not Named");
                    return;
                }

                e.Q<Label>("LabelTitle").text = elementReference.Name;
                e.AddToClassList("list-view-item");


                currentRemoveButtonAction ??= () => OnButtonRemoveElementClick(task, linkedBehaviours, listView, i, elementReference);

                var removeButton = e.Q<Button>("ButtonRemoveElement");
                removeButton.clickable.clicked -= currentRemoveButtonAction;
                removeButton.clickable.clicked += currentRemoveButtonAction;
            }

            listView = new ListView(linkedBehaviours, elementHeight, CreateElement, BindElement) { selectionType = SelectionType.None};
            parentContainer.Add(listView);
            return listView;

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