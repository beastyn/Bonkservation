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
        internal SensorSet SensorSet {get =>this.sensorSet; set { this.sensorSet = value;} }
        internal IndicatorSet IndicatorSet {get => this.indicatorSet; set { this.indicatorSet = value;} }
        internal TaskSet TaskSet { get => this.taskSet; set { this.taskSet = value;} }
        internal BehaviourSet BehaviourSet {get => this.behaviourSet; set { this.behaviourSet = value;} }

        [SerializeField] internal BrainDesignerData data;
        [SerializeField] internal string sceneReferencesObjName;
        [SerializeField] internal float taskTickRate = 0.1f;
        [SerializeField] internal float behaviourTickRate = 0.1f;

        internal List<Task> activeTasks = new();

        internal SceneReferences sceneReferences;

        bool brainDataMissing;

        SensorSet sensorSet;
        IndicatorSet indicatorSet;
        TaskSet taskSet;
        BehaviourSet behaviourSet;

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

            foreach (var sensor in this.sensorSet.list)
                sensor.Initialize(this, gameObject);

            foreach (var indicator in this.indicatorSet.list)
                indicator.Initialize();

            foreach (var task in this.taskSet.list)
                task.Initialize(this);

            foreach (var behaviour in this.behaviourSet.list)
                behaviour.Initialize(this, gameObject);

            //StartCoroutine(HeartbeatEvaluation());
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
            if (this.sensorSet != null && this.sensorSet.TryGetElementByName(sensorName, out foundSensor))
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
            if (this.indicatorSet != null && this.indicatorSet.TryGetElementByName(indicatorName, out foundIndicator))
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

            saveCache.behaviourSet = this.behaviourSet;
            saveCache.sensorSet = this.sensorSet;
            saveCache.indicatorSet= this.indicatorSet;
            saveCache.tasksSet = this.taskSet;

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
                tasksList.Append($"{task.Name}, ");
            helpBox.text = $"Detected Tasks: {tasksList}";
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

        /// <summary>Clear previous BrainDesigner data and refill it with data from saved asset.</summary>
        void UpdateBrainDesignerData(BrainDesignerData data)
        {
            this.sensorSet= data.sensorSet;
            this.indicatorSet= data.indicatorSet;
            this.taskSet = data.tasksSet;
            this.behaviourSet= data.behaviourSet;
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

            foreach (var sensor in this.sensorSet.list)
                sensor.Update();

            foreach (var indicator in this.indicatorSet.list)
                indicator.Update();

            //Collect active tasks after checking each of them.
            this.activeTasks.Clear();
            foreach (var task in this.taskSet.list)
                if (task.Update())
                    this.activeTasks.Add(task);

            //Predict next behaviour. If it is critical we interrupt current behaviour, otherwise preoceed with the current running one.
            var nextBehaviour = GetNextBehaviour();
            
            
            if(this.runningBehaviour == null)
                this.runningBehaviour = nextBehaviour; //If nothing is running, apply calculated Behaviour.
          
            if (nextBehaviour.critical && this.runningBehaviour != nextBehaviour && this.runningBehaviour.State == Node.NodeState.Running) 
                this.runningBehaviour.Interrupt();     //If predicted behaviour is critical and something already is running, interrupt the running one. We still will Tick it so it isproperly disabled.
            
            if (this.runningBehaviour.State != Node.NodeState.Running) 
                this.runningBehaviour = nextBehaviour; //Launch next behaviour is the running one finished (with any result).

            if (this.runningBehaviour?.behaviourSequence != null) 
                this.runningBehaviour.TickExecution(); //Tick chosen behaviour.
        }

        private Behaviour GetNextBehaviour()
        {
            //Choose Behaviour
            List<Behaviour> possibleBehaviours = new();
            foreach (var behaviour in this.behaviourSet.list)
            {

                var hashActiveTasks = this.activeTasks.ToHashSet();
                if (hashActiveTasks.Any(item => behaviour.ActivationTasks.Contains(item)))
                    possibleBehaviours.Add(behaviour);
            }

            return possibleBehaviours[Random.Range(0, possibleBehaviours.Count)];
        }
    }
}
