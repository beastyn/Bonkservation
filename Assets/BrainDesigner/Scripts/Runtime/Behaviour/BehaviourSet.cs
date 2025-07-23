using System;
using System.Collections.Generic;
using UnityEngine;
using BrainDesigner.Scripts.Utils;

namespace BrainDesigner.Scripts.Runtime
{
    [Serializable]
    public class BehaviourSet : Set<Behaviour>
    {

        [SerializeField] internal TaskSet tasks;
    }
}
