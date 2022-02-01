using System;
using System.Collections;
using System.Collections.Generic;

namespace MoonTrucker.GameWorld
{

    public class Level
    {
        public readonly int TimeLimit;
        public readonly int NumberOfTargets;

        public Level(int timeLimt, int numOfTargets)
        {
            TimeLimit = timeLimt;
            NumberOfTargets = numOfTargets;
        }
    }

    public class GameLevels
    {
        // TODO: Maybe make this an ArrayList
        private Level[] _levels;

        private int _currentLevel = 1;
        private bool _completedAllLevels = false;

        public GameLevels(Level[] levels)
        {
            if(levels.Length < 1)
            {
                throw new NotSupportedException("GameLevels must be initiaited with at least one Level");
            }

            _levels = new Level[levels.Length];
            for (int i = 0; i < levels.Length; i++)
            {
                _levels[i] = levels[i];
            }
        }

        public Level CurrentLevel => _levels[_currentLevel - 1];

        public bool AllLevelsComplete => _completedAllLevels;

        public TimeSpan CurrentLevelTimeLimit => TimeSpan.FromSeconds(CurrentLevel.TimeLimit);

        public int CurrentLevelNumber => _currentLevel;

        public Level NextLevel()
        {
            _currentLevel++;
            if(_currentLevel > _levels.Length)
            {
                _completedAllLevels = true;
                _currentLevel--; // Don't let the index get out of bounds
            }

            return CurrentLevel;
        }
    }
}
