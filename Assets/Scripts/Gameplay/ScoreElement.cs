namespace Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class ScoreElement
    {
        public string Name;
        public int Score;
        public List<BonkElement> Bonks = new();

        public ScoreElement(string name, int score, List<BonkElement> bonks) 
        {
            this.Name = name;
            this.Score = score;
            this.Bonks = bonks;
        }
    }

    [System.Serializable]
    public class BonkElement
    {
        public string Name;
        public int Times;

        public BonkElement(string name, int times)
        {
            this.Name = name;
            this.Times = times;

        }
    }
}
