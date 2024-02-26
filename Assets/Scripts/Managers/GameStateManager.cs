namespace Managers
{
    using Utils;
    using UnityEditor.Rendering;
    using UnityEngine;

    public class GameStateManager : MonoBehaviour
    {
        void OnEnable()
        {
            ManagersSOHolder.CameraEvent.CameraTransitionEndEvent += OnCameraTransitionEndEvent;
            PlayerSOHolder.PlayerSanity.EnergyChangeEvent += OnEnergyChangeEvent;
            ManagersSOHolder.TimeManagerSO.DayChangeEvent += OnDayChangeEvent;
            ManagersSOHolder.CameraEvent.CameraEndAnimationEvent += OnCameraEndAnimationEvent;
        }
        void OnDisable()
        {
            ManagersSOHolder.CameraEvent.CameraTransitionEndEvent -= OnCameraTransitionEndEvent;
            PlayerSOHolder.PlayerSanity.EnergyChangeEvent -= OnEnergyChangeEvent;
            ManagersSOHolder.TimeManagerSO.DayChangeEvent -= OnDayChangeEvent;
            ManagersSOHolder.CameraEvent.CameraEndAnimationEvent -= OnCameraEndAnimationEvent;
        }

        void Start()
        {
            ManagersSOHolder.GameStateSO.ResetGame();
            GamePause.SetPause(false);
        }

        void OnCameraTransitionEndEvent(int camNum)
        {
            if (camNum != -1)
            {
                switch (camNum)
                {
                    case 0:
                        {
                            if (ManagersSOHolder.GameStateSO.CurrentGameState != GameState.DayChanges) ManagersSOHolder.GameStateSO.SetGameState(GameState.Start); 
                            break;
                        }
                    case 1: ManagersSOHolder.GameStateSO.SetGameState(GameState.Room1); break;
                    default: break;
                }
            }
        }

        void OnDayChangeEvent(int dayNum)
        {
            ManagersSOHolder.GameStateSO.SetGameState(GameState.DayChanges);
            GamePause.SetPause(true);
        }

        void OnCameraEndAnimationEvent(GameState gameState)
        {
            if(gameState == GameState.DayChanges)
            {
                ManagersSOHolder.GameStateSO.SetGameState(GameState.Start);
                GamePause.SetPause(false);
            }
        }

        void OnEnergyChangeEvent(float value, bool isReastoring)
        {
            if (value == 0)
            {
                ManagersSOHolder.GameStateSO.SetGameState(GameState.GameOver);
                GamePause.SetPause(true);
            }
        }
    }
}