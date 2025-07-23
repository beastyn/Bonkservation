namespace Managers
{
    using Gameplay;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "ScoreManager", menuName = "Bonkservation/Managers/ScoreManager")]
    public class SOScoreManager : ScriptableObject
    {
        public UnityAction ScoreSavedEvent;
        public UnityAction NewHighScoreEvent;

        int daysEndured;
        Dictionary<string, int> bonkesDealt = new();
        ScoreElement lastScore;
        ScoreElement highScore;

        public int DaysEndured => this.daysEndured;
        public Dictionary<string, int> BonkesDealt => this.bonkesDealt;

        public ScoreElement LastScore => this.lastScore;
        public ScoreElement HighScore => this.highScore;

        public void ResetScore()
        {
            this.bonkesDealt.Clear();
            this.daysEndured = 0;
        }

        public void AddBonk(string idolName)
        {
            if (idolName == string.Empty) return;
            this.bonkesDealt.TryGetValue(idolName, out var currentCount);
            bonkesDealt[idolName] = currentCount + 1;
        }

        public void SetDaysEndured(int days) => this.daysEndured = days;

        public void ScoreSaved() => this.ScoreSavedEvent?.Invoke();

        public void SetLastScore(ScoreElement scoreElement) => this.lastScore = scoreElement;
        public void SetHighScore(ScoreElement scoreElement, bool notify = true)
        {
            this.highScore = scoreElement;
            if(notify) this.NewHighScoreEvent?.Invoke();
        }

    }
}