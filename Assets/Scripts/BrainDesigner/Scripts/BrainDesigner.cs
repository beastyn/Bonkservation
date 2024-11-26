using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace BrainDesigner.Scripts
{
    using Utils;
    using Runtime;
    using UnityEngine.UIElements;
    using System.Text;

    public class BrainDesigner : MonoBehaviour
    {
        internal BehaviourSet SelectedBehaviourSet => this.behaviourSets.GetValueOrDefault(this.selectedBehaviourSetId);
        internal SensorSet SelectedSensorSet => this.sensorSets.GetValueOrDefault(this.selectedSensorSetId);
        internal IndicatorSet SelectedIndicatorSet=> this.indicatorSets.GetValueOrDefault(this.selectedIndicatorSetId);
        internal TaskSet SelectedTaskSet => this.taskSets.GetValueOrDefault(this.selectedTaskSetId);

        [SerializeField] internal BrainDesignerData data;
        [SerializeField] internal string sceneReferencesObjName;
        [SerializeField] internal float taskTickRate = 0.1f;
        [SerializeField] internal float behaviourTickRate = 0.1f;

        internal int selectedBehaviourSetId;
        internal int selectedSensorSetId;
        internal int selectedIndicatorSetId;
        internal int selectedTaskSetId; 
        internal List<Task> activeTasks = new();

        internal SceneReferences sceneReferences;

        bool brainDataMissing;

        Dictionary<int, BehaviourSet> behaviourSets = new();
        Dictionary<int, SensorSet> sensorSets = new();
        Dictionary<int, IndicatorSet> indicatorSets = new();
        Dictionary<int, TaskSet> taskSets = new();
        BehaviourSet runningBehaviourSet;
        Behaviour runningBehaviour;

       

        void Awake()
        {
            this.Initialize();
            if (this.data != null)
            {
                Load(this.data);
                //UnlinkUtilityBehaviour();
            }
            else
                this.brainDataMissing = true;

            //CreateLocalConsiderationSets();
        }

        void Start()
        {
            if (this.data == null)
                return;

            foreach (var behaviour in this.SelectedBehaviourSet.list)
                behaviour.Initialize(this, gameObject);

            foreach (var sensor in this.SelectedSensorSet.list)
                sensor.Initialize(this, gameObject);

            foreach (var indicator in this.SelectedIndicatorSet.list)
                indicator.Initialize();

            foreach (var task in this.SelectedTaskSet.list)
                task.Initialize(this);
            
            //StartCoroutine(HeartbeatEvaluation());
            this.runningBehaviour = (Behaviour)this.SelectedBehaviourSet.list[0];
            StartCoroutine(TickedExecution());
        }
        /*
                private void Update()
                {
                    if (useUpdateAsTickRateForEvaluation)
                        Evaluate();

                    if (useUpdateAsTickRateForExecution)
                        Execute();
                }*/
        public bool TryGetSensorByName(string sensorName, out Sensor foundSensor)
        {
            if (this.SelectedSensorSet != null && this.SelectedSensorSet.TryGetElementByName(sensorName, out foundSensor))
                return true;
            else
            {
                foundSensor = null;
                return false;
            }
        }

        public bool TryGetSensoredObjects(Sensor sensor, out List<Transform> foundObjects)
        {
            sensor.TryGetSensoredObject(out foundObjects);
            return foundObjects != null;
        }

        public bool TryGetIndicatorByName(string indicatorName, out Indicator foundIndicator)
        {
            if (this.SelectedIndicatorSet != null && this.SelectedIndicatorSet.TryGetElementByName(indicatorName, out foundIndicator))
                return true;
            else
            {
                foundIndicator = null;
                return false;
            }
        }

        public void SetIndicatorValue(Indicator indicator, float newValue) => indicator.SetValue(newValue);

        public float GetIndicatorValue(Indicator indicator) => indicator.GetValue();

        internal void Initialize() => this.sceneReferences = GameObject.Find(sceneReferencesObjName)?.GetComponent<SceneReferences>();
       

#if UNITY_EDITOR
        internal void Save(string savePath)
        {
            //UnlinkUtilityBehaviour();

            var saveCache = ScriptableObject.CreateInstance<BrainDesignerData>();

            saveCache.behaviourSets = this.behaviourSets.Values.ToList();
            saveCache.sensorSets = this.sensorSets.Values.ToList();
            saveCache.indicatorSets= this.indicatorSets.Values.ToList();
            saveCache.tasksSets = this.taskSets.Values.ToList();

            AssetDatabase.DeleteAsset(savePath);
            AssetDatabase.CreateAsset(saveCache, savePath);
            AssetDatabase.SaveAssets();

            TryLoad(savePath);
        }

        internal bool TryLoad(string assetPath)
        {
            //Loading saved data
            BrainDesignerData loadedData = AssetDatabase.LoadAssetAtPath<BrainDesignerData>(assetPath);
            
            //If data is empty, do nothing
            if (loadedData == null)
                return false;

            //otherwise fill up sets data with data from asset.
            this.UpdateBrainDesignerData(loadedData);

            return true;
        }
        internal void UpdateGeneratedTasksInfo(HelpBox helpBox)
        {
            StringBuilder tasksList = new();

            foreach(var task in this.activeTasks)
                tasksList.AppendLine(task.Name);
            helpBox.text = $"Detected Tasks \n {tasksList}";
        }
#endif
        /// <summary>VLear old BrainDesigner Data and fill it with one from saved asset.</summary>
        internal void Load(BrainDesignerData dataToLoad)
        {
            if (dataToLoad == null)
                return;

            this.UpdateBrainDesignerData(dataToLoad);

            return;
        }

        internal Dictionary<int, BehaviourSet> GetBehaviourSets() => this.behaviourSets;
        internal Dictionary<int, SensorSet> GetSensorSets() => this.sensorSets;
        internal Dictionary<int, IndicatorSet> GetIndicatorSets() => this.indicatorSets;
        internal Dictionary<int, TaskSet> GetTaskSets() => this.taskSets;

        internal void AddBehaviourSet(int id, BehaviourSet behaviourSet)
        {
            this.behaviourSets.Add(id, behaviourSet);
            if (this.data != null)
                this.data.behaviourSets.Add(behaviourSet);
        }

        internal void AddSensorSet(int id, SensorSet sensorSet)
        {
            this.sensorSets.Add(id, sensorSet);
            if (this.data != null)
                this.data.sensorSets.Add(sensorSet);
        }

        internal void AddIndicatorSet(int id, IndicatorSet indicatorSet)
        {
            this.indicatorSets.Add(id, indicatorSet);
            if (this.data != null)
                this.data.indicatorSets.Add(indicatorSet);
        }

        internal void AddTaskSet(int id, TaskSet taskSet)
        {
            this.taskSets.Add(id, taskSet);
            if (this.data != null)
                this.data.tasksSets.Add(taskSet);
        }

        /// <summary>Clear previous BrainDesigner data and refill it with data from saved asset.</summary>
        void UpdateBrainDesignerData(BrainDesignerData data)
        {
            this.behaviourSets.Clear();
            this.sensorSets.Clear();
            this.indicatorSets.Clear();
            this.taskSets.Clear();

            if (data.behaviourSets == null) data.behaviourSets = new();
            if (data.sensorSets == null) data.sensorSets = new();
            if (data.indicatorSets == null) data.indicatorSets = new();
            if (data.tasksSets== null) data.tasksSets = new();

            var behaviourSetId = 0;
            var sensorSetId = 0;
            var indicatorSetId = 0;
            var taskSetId = 0;


            foreach (var behaviourSet in data.behaviourSets)
            {
                this.behaviourSets.Add(behaviourSetId, behaviourSet);
                behaviourSetId++;
            }
            foreach (var sensorSet in data.sensorSets)
            {
                this.sensorSets.Add(sensorSetId, sensorSet);
                sensorSetId++;
            }
            foreach (var indicatorSet in data.indicatorSets)
            {
                this.indicatorSets.Add(indicatorSetId, indicatorSet);
                indicatorSetId++;
            }
            foreach (var taskSet in data.tasksSets)
            {
                this.taskSets.Add(taskSetId, taskSet);
                taskSetId++;
            }

            this.data = data;
        }

        IEnumerator TickedExecution()
        {
            float tickRate = Mathf.Clamp(this.behaviourTickRate, 0.01f, 1f);
            while (true)
            {
                Execute();

                yield return new WaitForSecondsRealtime(tickRate);
            }
        }

        void Execute()
        {
            if (this.brainDataMissing)
                return;



            foreach (var sensor in this.SelectedSensorSet.list)
                sensor.Update();

            foreach (var indicator in this.SelectedIndicatorSet.list)
                indicator.Update();

            //Collect active tasks after checking each ofm them.
            this.activeTasks.Clear();
            foreach (var task in this.SelectedTaskSet.list)
                if (task.Update())
                    this.activeTasks.Add(task);

            if (this.runningBehaviour?.behaviourSequence != null)
                this.runningBehaviour.TickExecution(); // Only expensive when action nodes are expensive

        }
    }
}
