using Gameplay;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace Managers
{
    using Utils;
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] SOScoreManager scoreManagerSO;
        [SerializeField] GameObject scoreBoardUI;
        //[SerializeField] TimeManagerSO timeManagerSO;
        //[SerializeField] GameStateSO gameStateSO;

        void OnEnable() => GameStatesManager.GameOverEvent += this.OnGameOverEvent;
        void OnDisable() => GameStatesManager.GameOverEvent -= this.OnGameOverEvent;
        void Awake()
        {
            this.scoreManagerSO.ResetScore();

            var lastScore = FileHandler.ReadFromJSON<ScoreElement>("/LastScore.json");
            this.scoreManagerSO.SetLastScore(lastScore);

            var highScore = FileHandler.ReadFromJSON<ScoreElement>("/HighScore.json");
            this.scoreManagerSO.SetHighScore(highScore, false);
        }

        void OnGameOverEvent()
        {            

            this.scoreManagerSO.SetDaysEndured(TimeManager.CurrentDay);

            List<BonkElement> bonks = new();
            foreach (var bonk in this.scoreManagerSO.BonkesDealt)
                bonks.Add(new BonkElement(bonk.Key, bonk.Value));

            ScoreElement lastScore = new ScoreElement("Last Score", this.scoreManagerSO.DaysEndured, bonks);
            FileHandler.SaveToJSON(lastScore, "/LastScore.json");
            this.scoreManagerSO.SetLastScore(lastScore);

            if (this.scoreManagerSO.HighScore == null || this.scoreManagerSO.LastScore.Score > this.scoreManagerSO.HighScore.Score)
            {
                ScoreElement highScore = new ScoreElement("High Score", this.scoreManagerSO.DaysEndured, bonks);
                FileHandler.SaveToJSON(lastScore, "/HighScore.json");
            }

            this.scoreManagerSO.ScoreSaved();

            UIManager.OpenUI(this.scoreBoardUI);
        }
    }
}