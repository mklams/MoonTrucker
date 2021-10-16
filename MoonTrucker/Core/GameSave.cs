using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace MoonTrucker.Core
{
    public class Score
    {
        public int HitTotal;
        public string Name;
    }

    public class GameSave
    {
        private string _fileName = "StreetRacerSaveData.xml";
        public List<Score> HighScores { get; private set; }
        public GameSave()
        {
            HighScores = new List<Score>();
        }

        public void AddScore(Score score)
        {
            HighScores.Add(score);
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

        public void Save()
        {
            using (var writer = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
            {
                var serializer = new XmlSerializer(typeof(List<Score>));
                serializer.Serialize(writer, HighScores);
            }
        }
    }
}
