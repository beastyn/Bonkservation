namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class BonkerManager : MonoBehaviour
    {
        [SerializeField] GameObject bonker;
        [SerializeField] GameObject cursor;

        void OnEnable()
        {
            ManagersSOHolder.CameraEvent.CameraTransitionStartEvent += this.OnCameraTransitionStartEvent;
            ManagersSOHolder.CameraEvent.CameraTransitionEndEvent += this.OnCameraTransitionEndEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;

            PlayerSOHolder.SetPaw(this.bonker);
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
            this.cursor.SetActive(true);
        }
        void OnCameraTransitionStartEvent(int camNum)
        {
            this.bonker.SetActive(false);
            this.cursor.SetActive(false);
        }

        void OnCameraTransitionEndEvent(int camNum)
        {
            this.bonker.SetActive(camNum != 0);
            this.cursor.SetActive(camNum == 0);
        }

        void OnGameStateChangeEvent(GameState gameState)
        {
           /* if(gameState == GameState.GameOver)
                this.bonker.SetActive(false);*/

        }
    }
}
