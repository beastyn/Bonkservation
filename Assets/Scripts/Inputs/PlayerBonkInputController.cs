namespace Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.InputSystem;
    using Cinemachine;
    using UnityEngine.Events;
    using System.Diagnostics.Contracts;

    public class PlayerBonkInputController : MonoBehaviour
    {
        public static UnityAction<Vector2> MovementEvent;
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

        public void OnMovement(InputAction.CallbackContext value) => MovementEvent?.Invoke(value.ReadValue<Vector2>());

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
