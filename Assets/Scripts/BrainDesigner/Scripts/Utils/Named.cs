using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BrainDesigner.Scripts.Utils
{
    [Serializable]
    public abstract class Named
    {
        [SerializeField] string name;

        internal string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
