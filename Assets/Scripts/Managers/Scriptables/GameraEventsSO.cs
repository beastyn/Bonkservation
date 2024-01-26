namespace Managers.Cameras
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "CameraTracking", menuName = "Bonk/Managers/Cameras Tracking")]
    public class CameraEventsSO : ScriptableObject
    {
        public UnityAction<int> CameraNumRequestEvent;
        public UnityAction<int> CameraTransitionStartEvent;
        public UnityAction<int> CameraTransitionEndEvent;
        public UnityAction<GameState> CameraEndAnimationEvent;
    }
}