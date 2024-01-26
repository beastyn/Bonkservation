namespace Cameras
{
    using Cinemachine;
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UI.Cameras;

    [RequireComponent(typeof(CinemachineBrain))]
    public class CinemachineBlendChecker : MonoBehaviour
    {
        [SerializeField] CinemachineBrain cineMachineBrain;

        private bool wasBlendingLastFrame;

        void Awake()
        {
            cineMachineBrain ??= GetComponent<CinemachineBrain>();
        }
        void Start()
        {
            wasBlendingLastFrame = false;
        }

        void Update()
        {
            if (cineMachineBrain.IsBlending)
            {
                if (!wasBlendingLastFrame)
                {
                    ManagersSOHolder.CameraEvent.CameraTransitionStartEvent?.Invoke(this.TryGetCameraBNum());
                }

                wasBlendingLastFrame = true;
            }
            else
            {
                if (wasBlendingLastFrame)
                {
                    ManagersSOHolder.CameraEvent.CameraTransitionEndEvent?.Invoke(this.TryGetCameraBNum());
                    wasBlendingLastFrame = false;
                }
            }
        }

        int TryGetCameraBNum() => ((CinemachineVirtualCamera)cineMachineBrain.ActiveVirtualCamera).gameObject.GetComponent<IdolCameraUI>()?.GetCameraNum() ?? -1;

    }
}
