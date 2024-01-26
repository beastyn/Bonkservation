
namespace Managers
{
    using Gameplay;
    using Managers.Timer;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] ScoreManagerSO scoreManagerSO;
        [SerializeField] TimeManagerSO timeManagerSO;
        [SerializeField] GameStateSO gameStateSO;

        void OnEnable() => this.gameStateSO.GameStateChangedEvent += this.OnGameStateChangeEvent;
        void OnDisable() => this.gameStateSO.GameStateChangedEvent -= this.OnGameStateChangeEvent;

        void Start()
        {
            this.scoreManagerSO.ResetScore();

            var lastScore = FileHandler.ReadFromJSON<ScoreElement>("/LastScore.json");
            this.scoreManagerSO.SetLastScore(lastScore);

            var highScore = FileHandler.ReadFromJSON<ScoreElement>("/HightScore.json");
            this.scoreManagerSO.SetHighScore(highScore, false);
        }

        void OnGameStateChangeEvent(GameState gameState)
        {
            if (gameState != GameState.GameOver) return;

            this.scoreManagerSO.SetDaysEndured(this.timeManagerSO.CurrentDay);

            List<BonkElement> bonks = new();
            foreach (var bonk in this.scoreManagerSO.BonkesDealt)
                bonks.Add(new BonkElement(bonk.Key, bonk.Value));

            ScoreElement lastScore = new ScoreElement("Last Score", this.scoreManagerSO.DaysEndured, bonks);
            FileHandler.SaveToJSON(lastScore, "/LastScore.json");
            this.scoreManagerSO.SetLastScore(lastScore);

            if(this.scoreManagerSO.HighScore == null || this.scoreManagerSO.LastScore.Score > this.scoreManagerSO.HighScore.Score)
            {
                ScoreElement highScore = new ScoreElement("High Score", this.scoreManagerSO.DaysEndured, bonks);
                FileHandler.SaveToJSON(lastScore, "/HighScore.json");
            }

            this.scoreManagerSO.ScoreSaved();
        }
    }
}
