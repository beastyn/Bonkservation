namespace Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.InputSystem;
    using UnityEngine.Events;
    using System.Diagnostics.Contracts;

    public class PlayerInputReciever : MonoBehaviour
    {
        public static UnityAction<Vector2> MouseMovementEvent;
        public static UnityAction<Vector2> PlayerMovementEvent;
        public static UnityAction JumpEvent;
        public static UnityAction<bool> RunEvent;
        public static UnityAction MainActionEvent;
        public static UnityAction SecondActionEvent;
        public static UnityAction<float> SlotButton;
        public static UnityAction SettingsToggle;

        [SerializeField] PlayerInput playerInput;

        void OnEnable()
        {
            this.playerInput.currentActionMap.Enable();
        }

        void OnDisable()
        {
            this.playerInput.actions.FindActionMap("InputMap").Disable();
        }

        public void OnMouseMovement(InputAction.CallbackContext value)
        {
            MouseMovementEvent?.Invoke(value.ReadValue<Vector2>());
        }

        public void OnPlayerMovement(InputAction.CallbackContext value)
        {
            PlayerMovementEvent?.Invoke(value.ReadValue<Vector2>());
        }

/*        public void OnJump(InputAction.CallbackContext context)
        {
            PlayerMovementEvent?.Invoke(value.ReadValue<Vector2>());
        }
*/
        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
                RunEvent?.Invoke(true);
            else
                RunEvent?.Invoke(false);
        }

        public void OnBonk(InputAction.CallbackContext context)
        {
            if(context.performed)
                MainActionEvent?.Invoke();
        }
        public void OnDefend(InputAction.CallbackContext context)
        {
            if (context.performed)
                SecondActionEvent?.Invoke();
        }

        public void OnSlotButton(InputAction.CallbackContext context)
        {
            if(context.performed) SlotButton?.Invoke(context.ReadValue<float>());

        }

        public void OnMenuButton(InputAction.CallbackContext context)
        {
            if (context.performed) SettingsToggle?.Invoke();
        }
       
    }
}
