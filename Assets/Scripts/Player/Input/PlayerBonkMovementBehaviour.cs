namespace Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerBonkMovementBehaviour : MonoBehaviour
    {
        [SerializeField] PlayerBonkInputController input;

        //bool allowMovement = true;
        public Vector2 Velocity { get; private set; }

       /* void Start () {this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition); }*/

        void OnEnable()
        {
            PlayerBonkInputController.MovementEvent += OnMovementEvent;
            /*ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent += OnPrepareToSleep;
*/
            /*this.allowMovement = ManagersSOHolder.GameStateSO.CurrentGameState != GameState.DayChanges;*/
        }

        void OnDisable()
        {
            PlayerBonkInputController.MovementEvent -= OnMovementEvent;
      /*      ManagersSOHolder.GameStateSO.GameStateChangedEvent -= OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent -= OnPrepareToSleep;*/
        }

        void OnMovementEvent(Vector2 velocity)
        {
            /* if (!allowMovement) return;
             var worldPos = Camera.main.ScreenToWorldPoint(velocity);
             this.Velocity = velocity;
             this.transform.position = new Vector3(worldPos.x, worldPos.z);
             this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0f);*/


            // Raycast from the camera to the mouse position in the world
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a plane or collider
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 targetPosition = hit.point;
                targetPosition.y = this.gameObject.transform.position.y; // Set the Y position fixed

                // Move the object to the target position
                transform.position = targetPosition;
            }

        }

      /*  void OnGameStateChangeEvent(GameState gameState)
        {
            this.allowMovement = gameState != GameState.DayChanges;
        }

        void OnPrepareToSleep() => this.allowMovement = false;*/
    }
}
