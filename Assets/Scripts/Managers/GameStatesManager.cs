using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    using Gameplay;
    using Utils;

    public enum GameState
    {
        Running,
        Paused,
        GameOver
    }
    public class GameStatesManager : MonoBehaviour
    {
        public static UnityAction GameOverEvent;

        [SerializeField] SOEnergy playerStress;

        GameState currentGameState = GameState.Running;

        void OnEnable()
        {
            playerStress.FullEnergyEvent += OnFullEnergyEvent;            
        }

        void OnDisable()
        {
            playerStress.FullEnergyEvent -= OnFullEnergyEvent;
        }

        void Start()
        {
            TimeManager.SetPause(false);            
        }

        void OnFullEnergyEvent()
        {
            this.currentGameState = GameState.GameOver;
            GameOverEvent?.Invoke();
            TimeManager.SetPause(true);
        }

    }
}