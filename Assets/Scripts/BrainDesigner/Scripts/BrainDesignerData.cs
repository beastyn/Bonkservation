using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts
{
    using Runtime;

    public class BrainDesignerData : ScriptableObject
    {
        [SerializeField][HideInInspector] internal List<BehaviourSet> behaviourSets;
        [SerializeField][HideInInspector] internal List<SensorSet> sensorSets;
        [SerializeField][HideInInspector] internal List<IndicatorSet> indicatorSets;
        [SerializeField][HideInInspector] internal List<TaskSet> tasksSets;
    }
}
