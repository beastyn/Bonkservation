using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts
{
    using Runtime;

    [CreateAssetMenu(fileName = "Tasked Agent Data", menuName = "TaskedAgent/Tasked Agent Data")]
    public class TaskedAgentData : ScriptableObject
    {
        BehaviourSet behavioursSet;
        SensorSet sensorSet;
        TaskSet tasksSet;



        public BehaviourSet BehaviourSet => this.behavioursSet;
        public SensorSet SensorSet => this.sensorSet;
        public TaskSet TasksSet=> this.tasksSet;

        public void RememberBehaviourSet(BehaviourSet behaviourSet) => this.behavioursSet = behaviourSet;
        public void RememberSensorSet(SensorSet sensorSet) => this.sensorSet = sensorSet;
        public void RememberTaskSet(TaskSet taskSet) => this.tasksSet = taskSet;
    }
}
