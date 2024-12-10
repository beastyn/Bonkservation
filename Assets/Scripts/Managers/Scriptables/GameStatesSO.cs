namespace Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public enum GameState
    {
        Start,
        Room1,
        Room2,
        Room3,
        Room4,
        DayChanges,
        GameOver,
    }

    [CreateAssetMenu(fileName = "GameStates", menuName = "Bonk/Managers/GameStates")]
    public class GameStateSO : ScriptableObject
    {
        public UnityAction<GameState> GameStateChangedEvent;

        public GameState CurrentGameState => this.currentGameState;
        public GameState PreviousGameState => this.previousGameState;

        GameState currentGameState = GameState.Start;
        GameState previousGameState;

        public void SetGameState(GameState state)
        {
            this.previousGameState = this.currentGameState;
            this.currentGameState = state;
            this.GameStateChangedEvent?.Invoke(state);
        }

        public void ResetGame() => SetGameState(GameState.Start);
    }
}