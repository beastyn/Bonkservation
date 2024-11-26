using BrainDesigner.Scripts.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts
{
    public class TransformSceneReferences : SceneReferences
    {
        [SerializeField] List<Transform> transforms;

        protected override void RegisterCustomLists()
        {
            AddList(transforms);
        }
    }
}
