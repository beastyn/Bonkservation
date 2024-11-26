using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace BrainDesigner.Scripts
{
    using Runtime;

    public class TasksGeneratorManager : MonoBehaviour
    {
        [SerializeField] TaskedAgentData behaviourData;

        TaskSet currentTasks = new();
        TaskGenerator currentTaskGenerator;
        void Start()
        {/*
            if (!this.behaviourData.BehaviourSet.TryGetBehaviourByName("Run from Mane-san", out var runFromManesanBehaviour))
                return;
            if (!this.behaviourData.BehaviourSet.TryGetBehaviourByName("RoamRandom", out var roam))
                return;
            if (!this.behaviourData.BehaviourSet.TryGetBehaviourByName("Mischieve", out var mischieve))
                return;
*/
          /*  var roamingTask = new Task("Roam", new List<Behaviour> { roam, mischieve });
            var runFromManesanTask = new Task("Run from Mane-san", new List<Behaviour>(){ runFromManesanBehaviour });


            this.currentTaskGenerator = new TaskGenerator();
            this.currentTaskGenerator.InitGenerator(new Dictionary<Task, List<Sensor>>() { { runFromManesanTask, this.behaviourData.SensorSet.List }}, new Dictionary<Task, Func<bool>>(){ { roamingTask, Always } } );

            this.currentTasks.List = this.currentTaskGenerator.GeneratedTasks;
            this.behaviourData.RememberTaskSet(this.currentTasks);*/
        }

        void Update()
        {
           /* this.currentTaskGenerator.Update();
            this.currentTasks.list = this.currentTaskGenerator.GeneratedTasks;
            this.behaviourData.RememberTaskSet(this.currentTasks);*/
        }

        bool Always() => true;
    }
}