using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif

namespace BrainDesigner.Scripts
{
    using Runtime;

    public class BrainDesigner : MonoBehaviour
    {
        /// <summary>Notification about new action start. Return first as previous action, second as next action</summary>
        public System.Action<string, string> ActionChangeEvent;
        public string CurrentBehaciourName => this.currentBehaviourName;

        internal SensorSet SensorSet { get => this.sensorSet; set { this.sensorSet = value; } }
        internal IndicatorSet IndicatorSet { get => this.indicatorSet; set { this.indicatorSet = value; } }
        internal TaskSet TaskSet { get => this.taskSet; set { this.taskSet = value; } }
        internal BehaviourSet BehaviourSet { get => this.behaviourSet; set { this.behaviourSet = value; } }

        [SerializeField] internal BrainDesignerData data;
        [SerializeField] internal string sceneReferencesObjName;
        [SerializeField] internal float taskTickRate = 0.1f;
        [SerializeField] internal float behaviourTickRate = 0.1f;

        internal List<Task> activeTasks = new();
        internal List<Behaviour> fallOffBehaviours = new();

        internal SceneReferences sceneReferences;

        bool brainDataMissing;
        string currentBehaviourName; 

        SensorSet sensorSet;
        IndicatorSet indicatorSet;
        TaskSet taskSet;
        BehaviourSet behaviourSet;

        bool isBrainUp = true;
        Behaviour previousBehaviour;
        Behaviour runningBehaviour;


        void OnEnable()
        {
            if(!this.isBrainUp) this.Start();
            
        }

        void OnDisable()
        {
            StopAllCoroutines();
            this.runningBehaviour.Interrupt();
            this.runningBehaviour = null;
            this.isBrainUp= false;
        }

        void Awake()
        {
            this.Initialize();
            if (this.data != null)
            {
                this.Load(this.data);
                this.LocalizeBrainData();
            }
            else
                this.brainDataMissing = true;

            //CreateLocalConsiderationSets();
        }

        void Start()
        {
            if (this.data == null)
                return;

            this.isBrainUp = true;

            foreach (var sensor in this.sensorSet.list)
                sensor.Initialize(this, gameObject);

            foreach (var indicator in this.indicatorSet.list)
                indicator.Initialize(this);

            foreach (var behaviour in this.behaviourSet.list)
            {
                behaviour.Initialize(this, gameObject);
                if (behaviour.isDefault) this.fallOffBehaviours.Add(behaviour);
            }

            foreach (var task in this.taskSet.list)
                task.Initialize(this);

            if (this.fallOffBehaviours.Count == 0)
            {
                Debug.LogError("Need at least one default behaviour");
                return;
            }

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
        #region Public API
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

        public string GetRunningBehaviourName() => this.runningBehaviour?.Name ?? string.Empty;

        public string GetPreviousBehaviour() => this.previousBehaviour?.Name?? string.Empty;

        #endregion

        internal void Initialize() => this.sceneReferences = GameObject.Find(sceneReferencesObjName)?.GetComponent<SceneReferences>();
       

#if UNITY_EDITOR
        internal void Save(string savePath)
        {
            this.LocalizeBrainData();

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

            //Remember what is running now.
            this.previousBehaviour = this.runningBehaviour;
            var previousAction = this.runningBehaviour?.behaviourSequence.RunningAction ?? null;
            //Predict next behaviour. If it is critical we interrupt current behaviour, otherwise preoceed with the current running one.
            var nextBehaviour = GetNextBehaviour();
            
            
            if(this.runningBehaviour == null)
                this.runningBehaviour = nextBehaviour; //If nothing is running, apply calculated Behaviour.

            var nextMoreCritical = nextBehaviour.critical && (!runningBehaviour.critical || nextBehaviour.baseScore > runningBehaviour.baseScore); 
          
            if (this.runningBehaviour != nextBehaviour && this.runningBehaviour.State == Node.NodeState.Running && nextMoreCritical) 
                this.runningBehaviour.Interrupt();     //If predicted behaviour is critical and something already is running, interrupt the running one. We still will Tick it so it isproperly disabled.
            
            if (this.runningBehaviour.State != Node.NodeState.Running) 
                this.runningBehaviour = nextBehaviour; //Launch next behaviour is the running one finished (with any result).

            if (this.runningBehaviour?.behaviourSequence != null)
            {
                this.runningBehaviour.TickExecution();
            }

            this.currentBehaviourName = this.runningBehaviour.Name;
            //Notify only if behaviours node was changed
            var runningAction = this.runningBehaviour?.behaviourSequence.RunningAction ?? null;
            if (previousAction?.GetType().Name != runningAction?.GetType().Name) this.ActionChangeEvent?.Invoke(previousAction?.GetType().Name, runningAction?.GetType().Name);
        }

        Behaviour GetNextBehaviour()
        {
            //Choose Behaviour
            List<Behaviour> possibleBehaviours = new();
            foreach (var behaviour in this.behaviourSet.list)
            {

                var hashActiveTasks = this.activeTasks.ToHashSet();

                var lastBrhaviourScore = 0f;
                if (hashActiveTasks.Any(item => behaviour.ActivationTasks.Contains(item)))
                {
                    if(behaviour.baseScore >= lastBrhaviourScore)
                        possibleBehaviours.Add(behaviour);
                    lastBrhaviourScore = behaviour.baseScore;
                }
            }

            if (possibleBehaviours.Count == 0)
                possibleBehaviours = this.fallOffBehaviours;
            return possibleBehaviours[Random.Range(0, possibleBehaviours.Count)];
        }

        void LocalizeBrainData()
        {
            /*this.sensorSet = data.sensorSet;
            this.indicatorSet = data.indicatorSet;
            this.taskSet = data.tasksSet;
            this.behaviourSet = data.behaviourSet;
            this.data = data;*/

            //Create local instances for each brain entity, as currently there is only one instance that was created from Brain Designer Editor.

            //Create copy of sensors

            SensorSet newSensorSet = new();
            IndicatorSet newIndicatorSet = new();
            TaskSet newTaskSet = new();
            BehaviourSet newBehaviourSet = new();

            newSensorSet.Name= this.sensorSet.Name;
            newIndicatorSet.Name = this.indicatorSet.Name;
            newTaskSet.Name = this.taskSet.Name;
            newBehaviourSet.Name = this.behaviourSet.Name;


            foreach (var sensor in this.sensorSet.list)            
            {
                var newSensor = sensor.Clone();
                ((Sensor)newSensor).Name = sensor.Name;
                newSensorSet.list.Add((Sensor)newSensor);
            }
            this.sensorSet= newSensorSet;

            foreach (var indicator in this.indicatorSet.list)
            {
                var newIndicator = indicator.Clone();
                ((Indicator)newIndicator).Name = indicator.Name;
                newIndicatorSet.list.Add((Indicator)newIndicator);
            }
            this.indicatorSet= newIndicatorSet;

            foreach (var behaviour in this.behaviourSet.list)
            {
                var newBehaviour = behaviour.Clone();
                ((Behaviour)newBehaviour).Name = behaviour.Name;
                newBehaviourSet.list.Add((Behaviour)newBehaviour);
            } 
            this.behaviourSet= newBehaviourSet;

            foreach (var task in this.taskSet.list)
            {
                task.SetBrainDesigner(this);
                var newTask = task.Clone();
                ((Task)newTask).Name = task.Name;
                newTaskSet.list.Add((Task)newTask);
            }           
            this.taskSet= newTaskSet;            
        }
    }
}
