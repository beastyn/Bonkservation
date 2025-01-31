using KinematicCharacterController.Examples;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputController : MonoBehaviour
    { 
        public GameObject Avatar;
        public PlayerMovementController Player;       
        public PlayerCameraController  PlayerCamera;

        Vector2 mouseDelta = Vector2.zero;
        Vector2 moveDelta = Vector2.zero;
        bool isRunning = false;

        void OnEnable()
        {
            PlayerInputReciever.MouseMovementEvent += OnMouseMovementEvent;
            PlayerInputReciever.PlayerMovementEvent += OnPlayerMovementEvent;
            PlayerInputReciever.RunEvent += OnRunEvent;
            
        }

        void OnDisable()
        {
            PlayerInputReciever.MouseMovementEvent -= OnMouseMovementEvent;
            PlayerInputReciever.PlayerMovementEvent -= OnPlayerMovementEvent;

        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            PlayerCamera.SetFollowTransform(Player.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            PlayerCamera.IgnoredColliders.Clear();
            PlayerCamera.IgnoredColliders.AddRange(Avatar.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (PlayerCamera.RotateWithPhysicsMover && Player.Motor.AttachedRigidbody != null)
            {
                PlayerCamera.PlanarDirection = Player.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * PlayerCamera.PlanarDirection;
                PlayerCamera.PlanarDirection = Vector3.ProjectOnPlane(PlayerCamera.PlanarDirection, Player.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera

            float mouseLookAxisUp = this.mouseDelta.y;
            float mouseLookAxisRight = this.mouseDelta.x;
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = 0f;//-Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            PlayerCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            /*            if (Input.GetMouseButtonDown(1))
                        {
                            PlayerCamera.TargetDistance = (PlayerCamera.TargetDistance == 0f) ? PlayerCamera.DefaultDistance : 0f;
                        }*/
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = this.moveDelta.y;
            characterInputs.MoveAxisRight = this.moveDelta.x;
            characterInputs.CameraRotation = PlayerCamera.Transform.rotation;
            characterInputs.Run = this.isRunning;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

            // Apply inputs to character
            Player.SetInputs(ref characterInputs);
        }

        void OnMouseMovementEvent(Vector2 mouseDelta) => this.mouseDelta = mouseDelta;
        void OnPlayerMovementEvent(Vector2 moveDelta) => this.moveDelta = moveDelta;
        void OnRunEvent(bool isRunning) => this.isRunning = isRunning;
    }
}