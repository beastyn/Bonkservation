namespace Managers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    [Serializable]
    [CreateAssetMenu(fileName = "Lights", menuName = "Bonk/Lights")]
    public class LightPresetSO : ScriptableObject
    {
        public Gradient DirectinalColor;
        public Gradient[] RoomsColor;


    }
}
