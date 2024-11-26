using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrainDesigner.Scripts.Runtime
{
    public class TaskGenerator
    {
        internal List<Task> GeneratedTasks => this.generatedTasks;

        protected List<Task> generatedTasks = new();

        Dictionary<Task, List<Sensor>> sensorsToCheckforTask = new();
        Dictionary<Task, Func<bool>> conditionsToCjeckForTask = new();

        internal void InitGenerator(Dictionary<Task,List<Sensor>> sensorsToCheckforTask, Dictionary<Task, Func<bool>>conditionsToCjeckForTask = null)
        {
            this.sensorsToCheckforTask = sensorsToCheckforTask;
            this.conditionsToCjeckForTask=conditionsToCjeckForTask;
        }

        internal void Update() 
        {
            this.generatedTasks.Clear();

            //Check all provided sensor to add a related task
            foreach (var sensorToTask in this.sensorsToCheckforTask)
            {
                foreach (var sensor in sensorToTask.Value)
                    if (sensor.TryGetSensoredObject(out var sensorObj))
                        this.generatedTasks.Add(sensorToTask.Key);
            }

            foreach (var conditionToTask in this.conditionsToCjeckForTask)
                if (conditionToTask.Value.Invoke())
                    this.generatedTasks.Add(conditionToTask.Key);
            this.OnUpdate();

            foreach (var task in this.GeneratedTasks)
            {
                Debug.Log($"{task.Name}\n");
            }
            
        }

        protected void OnUpdate() { }

    }
}
