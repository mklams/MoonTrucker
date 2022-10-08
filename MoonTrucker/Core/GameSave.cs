using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;

namespace MoonTrucker.Core
{
    public class Score
    {
        public long Value;
        public string PlayerName;
        public GameMode Mode;
        public bool LowerIsBetter;

        public Score(long score, string name, GameMode mode)
        {
            Value = score;
            PlayerName = name;
            Mode = mode;
        }
        public Score() { }
    }

    public class HighScores
    {
        private const int TOTAL_HIGH_SCORES = 15;

        public List<Score> Scores { get; private set; }

        public HighScores(List<Score> previousScores)
        {
            Scores = previousScores;
        }

        public HighScores() : this(new List<Score>()) { }

        public int HighScoreCountForMode(GameMode mode)
        {
            return Scores.Where(score => score.Mode == mode).Count();
        }

        public List<Score> GetAllTopScores()
        {
            List<Score> topScores = new List<Score>();
            foreach(GameMode mode in Enum.GetValues(typeof(GameMode)))
            {
                topScores.AddRange(GetTopScoresForMode(mode));
            }

            return topScores;
        }

        public List<Score> GetTopScoresForMode(GameMode mode)
        {
            var scoreForMode = from score in Scores
                    where score.Mode == mode
                    orderby score.Value descending
                    select score;

            var orderedScores = IsLowerBetter(mode) ? scoreForMode.OrderBy(score => score.Value) : scoreForMode.OrderByDescending(score => score.Value);
            return orderedScores.Take(TOTAL_HIGH_SCORES).ToList();
        }

        public bool IsATopScore(long score, GameMode mode)
        {
            if(score == 0)
            {
                return false;
            }

            if(HighScoreCountForMode(mode) > TOTAL_HIGH_SCORES)
            {
                var scores = GetTopScoresForMode(mode);
                return IsLowerBetter(mode) ? scores.Last().Value > score : scores.Last().Value < score;
            }

            return true;
        }

        public void AddScore(Score score)
        {
            Scores.Add(score);
        }

        private bool IsLowerBetter(GameMode mode)
        {
            // TODO: HighScore should know need to know which modes use LowerIsBetter
            return mode == GameMode.Arcade;
        }
    }

    // TODO: Calls this something better...GameSaveManager?
    public class GameSave
    {
        private string _fileName = "StreetRacerSaveData.xml";
        public List<Score> HighScores { get; private set; }

        

        public GameSave()
        {
            HighScores = new List<Score>();
        }

        public List<Score> Load()
        {
            if(!File.Exists(_fileName))
            {
                return HighScores;
            }

            using(var reader = new StreamReader(new FileStream(_fileName, FileMode.Open)))
            {
                var serializer = new XmlSerializer(typeof(List<Score>));
                HighScores = (List<Score>)serializer.Deserialize(reader);
                return HighScores;
            }
        }

        // TODO: Make a GameSaveData object
        public void Save(List<Score> scores)
        {
            using (var writer = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
            {
                var serializer = new XmlSerializer(typeof(List<Score>));
                serializer.Serialize(writer, scores);
            }
        }
    }
}
