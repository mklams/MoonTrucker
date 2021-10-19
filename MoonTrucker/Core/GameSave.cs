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
        public int HitTotal;
        public string Name;

        public Score(int pointTotal, string name)
        {
            HitTotal = pointTotal;
            Name = name;
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

        public List<Score> GetTopScores(int topNumber = TOTAL_HIGH_SCORES)
        {
            return (from score in Scores
                    orderby score.HitTotal descending
                    select score).Take(topNumber).ToList();
        }

        public bool IsATopScore(int score)
        {
            if(score == 0)
            {
                return false;
            }

            if(Scores.Count > TOTAL_HIGH_SCORES)
            {
                var scores = GetTopScores(TOTAL_HIGH_SCORES);
                return scores.Last().HitTotal < score;
            }

            return true;
        }

        public void AddScore(Score score)
        {
            Scores.Add(score);
            if (Scores.Count > TOTAL_HIGH_SCORES)
            {

                Scores = GetTopScores(TOTAL_HIGH_SCORES);
            }
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
