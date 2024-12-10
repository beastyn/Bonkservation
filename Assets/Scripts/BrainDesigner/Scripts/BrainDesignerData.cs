using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts
{
    using Runtime;

    public class BrainDesignerData : ScriptableObject
    {
        [SerializeField][HideInInspector] internal BehaviourSet behaviourSet;
        [SerializeField][HideInInspector] internal SensorSet sensorSet;
        [SerializeField][HideInInspector] internal IndicatorSet indicatorSet;
        [SerializeField][HideInInspector] internal TaskSet tasksSet;
    }
}
