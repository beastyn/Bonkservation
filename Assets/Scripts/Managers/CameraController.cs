namespace Managers.Cameras
{
    using Cinemachine;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    using DG.Tweening;
    using System;

    public class CameraController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera mainCamera;
        [SerializeField] CinemachineVirtualCamera[] idolsCameras;
        [SerializeField] Volume ppVolume;
 
        [SerializeField] float gameOverSaturation = -100f;
        [SerializeField] float gameOverExposure = -4f;
        [SerializeField] float gameOvervIntensity = 0.3f;
        [SerializeField] float gameOverTweenTime = 3f;

        [SerializeField] float closedEyesSaturation = -100f;
        [SerializeField] float closedEyesExposure = -10f;
        [SerializeField] float closedEyesIntensity = 1f;
        [SerializeField] float closedEyesTweenTime = 2f;

        [SerializeField] float blinkSaturation = -100f;
        [SerializeField] float blinkExposure = -6f;
        [SerializeField] float blinkIntensity = 0.8f;
        [SerializeField] float blinkTweenTime = 0.1f;

        CinemachineVirtualCamera activeCamera;

        ColorAdjustments coloring;
        Vignette vignette;

        void OnEnable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent += OnCameraNumRequestEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent += OnPrepareToSleep;
        }

        void OnDisable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent -= OnCameraNumRequestEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent -= OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent -= OnPrepareToSleep;
        }

        void Start()
        {
            this.ppVolume.profile.TryGet(out this.coloring);
            this.ppVolume.profile.TryGet(out this.vignette);

            this.coloring.saturation.value = -100f;
            this.coloring.postExposure.value = -4f;
            this.vignette.intensity.value = 0.3f;

            this.activeCamera = this.mainCamera;

            DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, 0f, 5);
            DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, 0f, 5);
            DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, 0f, 5);
        }

        void Update()
        {
           /* if (this.requestStartAnimation)
            {
                DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, 0f, 3).SetUpdate(true);
                DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, 0f, 3).SetUpdate(true);
                DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, 0f, 3).SetUpdate(true).OnComplete(() => this.requestGameOverAnimation = requestStartAnimation);
            }
            if(this.requestGameOverAnimation)
            {
                DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, -100f, 3).SetUpdate(true);
                DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, -4f, 3).SetUpdate(true);
                DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, 0.3f, 3).SetUpdate(true).OnComplete(() => this.requestGameOverAnimation = false);
            }*/

        }

        void OnCameraNumRequestEvent(int camNum)
        {
            if (camNum == 0 && this.activeCamera != null)
            {
                this.activeCamera.gameObject.SetActive(false);
                this.mainCamera.gameObject.SetActive(true);
                return;
            }
            this.idolsCameras[camNum-1].gameObject.SetActive(true);
            this.mainCamera.gameObject.SetActive(false);
            this.activeCamera = this.idolsCameras[camNum-1];
        }

        public void OnPrepareToSleep()
        {
            this.BlinkSequence(this.closedEyesTweenTime);
        }

        void OnGameStateChangeEvent(GameState gameState)
        {
            if (gameState == GameState.DayChanges)
            {
                if (this.activeCamera != this.mainCamera) ManagersSOHolder.CameraEvent.CameraNumRequestEvent?.Invoke(0);
            }

            if (gameState == GameState.GameOver)
            {
                DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, this.gameOverSaturation, this.gameOverTweenTime).SetUpdate(true);
                DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, this.gameOverExposure, this.gameOverTweenTime).SetUpdate(true);
                DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, this.gameOvervIntensity, this.gameOverTweenTime).SetUpdate(true);
            }
        }

        void CloseEyesTween(float speed)
        {
            DOTween.Kill(this);

            DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, this.closedEyesIntensity, speed).SetUpdate(true);
            DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, this.closedEyesSaturation, speed).SetUpdate(true);
            DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, this.closedEyesExposure, speed + 0.5f).SetUpdate(true);
        }

        void BlinkEyesTween()
        {
            DOTween.Kill(this);

            DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, this.blinkIntensity, this.blinkTweenTime).SetUpdate(true);
            DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, this.blinkSaturation, this.blinkTweenTime).SetUpdate(true);
            DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, this.blinkExposure, this.blinkTweenTime + 1f).SetUpdate(true);
        }

        void BlinkSequence(float closeEyesSpeed)
        {
            DOTween.KillAll();

            var sequence = DOTween.Sequence();

            sequence.Append(DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, this.closedEyesIntensity, closeEyesSpeed).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, this.closedEyesSaturation, closeEyesSpeed).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, this.closedEyesExposure, closeEyesSpeed).SetUpdate(true));

            sequence.Append(DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, this.blinkIntensity, this.blinkTweenTime).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, this.blinkSaturation, this.blinkTweenTime).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, this.blinkExposure, this.blinkTweenTime).SetUpdate(true));

            sequence.Append(DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, this.closedEyesIntensity, this.blinkTweenTime).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, this.closedEyesSaturation, this.blinkTweenTime).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, this.closedEyesExposure, this.blinkTweenTime).SetUpdate(true));

            sequence.Append(DOTween.To(() => this.vignette.intensity.value, x => this.vignette.intensity.value = x, 0f, closeEyesSpeed).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.saturation.value, x => this.coloring.saturation.value = x, 0f, closeEyesSpeed).SetUpdate(true))
                    .Join(DOTween.To(() => this.coloring.postExposure.value, x => this.coloring.postExposure.value = x, 0f, closeEyesSpeed).SetUpdate(true)).OnComplete(() => ManagersSOHolder.CameraEvent.CameraEndAnimationEvent?.Invoke(GameState.DayChanges));

            sequence.Play().SetUpdate(true);
        }
    }
}
