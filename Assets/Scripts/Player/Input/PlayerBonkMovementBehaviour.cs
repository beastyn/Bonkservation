namespace Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerBonkMovementBehaviour : MonoBehaviour
    {
        [SerializeField] PlayerInputReciever input;
        [SerializeField] float followMouseSpeed = 1f;

        //bool allowMovement = true;
        int layerNumber = 3;
        int layerMask;

        bool isMouseMoved = false;
        Vector3 targetPosition;

        public Vector2 Velocity { get; private set; }

        void Start()
        {
            this.layerMask = 1 << this.layerNumber;
            this.targetPosition = transform.position;
        }

        void OnEnable()
        {
            PlayerInputReciever.MouseMovementEvent += OnMovementEvent;
            /*ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent += OnPrepareToSleep;
*/
            /*this.allowMovement = ManagersSOHolder.GameStateSO.CurrentGameState != GameState.DayChanges;*/
        }

        void OnDisable()
        {
            PlayerInputReciever.MouseMovementEvent -= OnMovementEvent;
      /*      ManagersSOHolder.GameStateSO.GameStateChangedEvent -= OnGameStateChangeEvent;
            ManagersSOHolder.TimeManagerSO.PrepareToSleepEvent -= OnPrepareToSleep;*/
        }

        void OnMovementEvent(Vector2 velocity)
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a plane or collider
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                this.targetPosition = hit.point;
                this.targetPosition.y = this.gameObject.transform.position.y; // Set the Y position fixed
            }
        }

        void FixedUpdate()
        {

            this.transform.position = Vector3.Lerp(this.transform.position, this.targetPosition, this.followMouseSpeed);

        }

        /*  void OnGameStateChangeEvent(GameState gameState)
          {
              this.allowMovement = gameState != GameState.DayChanges;
          }

          void OnPrepareToSleep() => this.allowMovement = false;*/
    }
}
