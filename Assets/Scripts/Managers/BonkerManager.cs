namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class BonkerManager : MonoBehaviour
    {
        [SerializeField] GameObject bonker;

        void OnEnable()
        {
            ManagersSOHolder.CameraEvent.CameraTransitionStartEvent += this.OnCameraTransitionStartEvent;
            ManagersSOHolder.CameraEvent.CameraTransitionEndEvent += this.OnCameraTransitionEndEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;
        }
        void OnDisable()
        {
            ManagersSOHolder.CameraEvent.CameraTransitionStartEvent -= this.OnCameraTransitionStartEvent;
            ManagersSOHolder.CameraEvent.CameraTransitionEndEvent -= this.OnCameraTransitionEndEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent -= OnGameStateChangeEvent;
        }


        void Start()
        {
            this.bonker.SetActive(false);
        }
        void OnCameraTransitionStartEvent(int camNum)
        {
            if (camNum == 0)
                this.bonker.SetActive(false);
        }

        void OnCameraTransitionEndEvent(int camNum)
        {
            if(camNum != 0)
                this.bonker.SetActive(true);
        }

        void OnGameStateChangeEvent(GameState gameState)
        {
           /* if(gameState == GameState.GameOver)
                this.bonker.SetActive(false);*/

        }


    }
}
