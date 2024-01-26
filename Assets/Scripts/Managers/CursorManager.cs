namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CursorManager : MonoBehaviour
    {
        void OnEnable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent += OnCameraNumRequestEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent += OnGameStateChangeEvent;
            ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent += OnScoreSavedEvent;
        }

        void OnDisable()
        {
            ManagersSOHolder.CameraEvent.CameraNumRequestEvent -= OnCameraNumRequestEvent;
            ManagersSOHolder.GameStateSO.GameStateChangedEvent -= OnGameStateChangeEvent;
            ManagersSOHolder.ScoreManagerSO.ScoreSavedEvent -= OnScoreSavedEvent;
        }

        void OnCameraNumRequestEvent(int camNum)
        {
          /*  if (camNum == 0 && this.managers.GameStateSO.CurrentGameState != GameState.DayChanges)
                Cursor.visible = true;
            if (camNum != 0)
                Cursor.visible = false;*/
        }

        void OnGameStateChangeEvent(GameState gameState)
        {

            Cursor.visible = Cursor.visible != true && gameState != GameState.DayChanges && gameState != GameState.Room1;
        }

        void OnScoreSavedEvent()
        {
            Cursor.visible = true;
        }

    }
}
