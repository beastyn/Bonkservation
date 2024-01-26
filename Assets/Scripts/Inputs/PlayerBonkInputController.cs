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

        public void OnMovement(InputAction.CallbackContext value) => MovementEvent?.Invoke(value.ReadValue<Vector2>());

        public void OnBonk(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
               MainActionEvent?.Invoke();
            }
        }
        public void OnDefend(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SecondActionEvent?.Invoke();
            }
        }
    }
}
