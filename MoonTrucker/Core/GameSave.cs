using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace MoonTrucker.Core
{

    [Serializable]
    public struct SaveGame
    {
        List<Score> HighScores;
    }

    public class Score
    {
        int HitTotal;
        string Name;
    }

    public class GameSave
    {
        public GameSave()
        {
        }

        public void SaveToDevice()
        {
            
        }
    }
}
