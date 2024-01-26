namespace Player
{
    using Managers;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerBonkMovementBehaviour : MonoBehaviour
    {
        [SerializeField] PlayerBonkInputController input;

        bool allowMovement = true;
        public Vector2 Velocity { get; private set; }

        void OnEnable()
        {
            this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PlayerBonkInputController.MovementEvent += OnMovementEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent += OnPrepareToSleep;
        }

        void OnDisable()
        {
            PlayerBonkInputController.MovementEvent -= OnMovementEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent -= OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent -= OnPrepareToSleep;
        }

        void OnMovementEvent(Vector2 velocity)
        {
            if (!allowMovement) return;
            var worldPos = Camera.main.ScreenToWorldPoint(velocity);
            this.Velocity = velocity;
            this.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
        }

        void OnGameStateChangeEvent(GameState gameState)
        {
            this.allowMovement = gameState == GameState.DayChanges || gameState == GameState.GameOver;
        }

        void OnPrepareToSleep() => this.allowMovement = false;
    }
}
