using System;
using System.Collections;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class Level: IDrawable, IObserver<GameTarget>
    {
        public readonly LevelConfig Config;
        private GameMap _map;
        private PropFactory _propFactory;
        private int _score;
        private int _targetsHit;
        private Timer _timer;

        public TimeSpan TimeLimit => TimeSpan.FromSeconds(Config.TimeLimit);
        public bool LevelIsFinished => _map.IsPlayerInWinZone();
        public int TimeLeftInSeconds => _timer.GetTimeInSeconds();
        public bool AllTargetsCollected => _targetsHit >= _map.GetNumberOfTargets();

        public Level(LevelConfig config, float tileWidth, PropFactory propFactory)
        {
            _propFactory = propFactory;
            Config = config;
            _map = new GameMap(Config, tileWidth, _propFactory);
            _score = 0;
            _targetsHit = 0;
            _timer = new Timer(TimeLimit);
        }

        public void Load()
        {
            _map.Load();
            _map.SubscribeToTargets(this);
            _map.SubscribeToTargets(_timer);
        }

        public int GetScore()
        {
            return _score;
        }

        public Vector2 GetFinishPosition()
        {
            return _map.GetFinishPosition();
        }

        public Vector2 GetTargetPosition()
        {
            // TODO: Actually get the position
            return new Vector2(0f, 0f);
        }

        public Vector2 GetStartPosition()
        {
            return _map.GetStartPosition();
        }      

        public bool IsTimeUp()
        {
            return _timer.IsTimerUp();
        }

        public void Draw()
        {
            _map.Draw();
        }

        public void Update(GameTime gameTime)
        {
            updateFinish();
            _timer.Update(gameTime);
        }

        private void updateFinish()
        {
            if (AllTargetsCollected)
            {
                _map.ActivateFinish();
            }
        }

        private void targetHit()
        {
            _targetsHit++;
            _score += 100;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(GameTarget value)
        {
            targetHit();
        }
    }


    public class LevelConfig
    {
        public readonly int TimeLimit;
        public readonly string MapName;

        public LevelConfig(int timeLimt, string mapName)
        {
            TimeLimit = timeLimt;
            MapName = mapName;
        }
    }

    public class GameLevels
    {
        private LevelConfig[] _levelsConfig = new LevelConfig[3]
        {
            new LevelConfig(15, "MoonTrucker.GameWorld.Level.txt"),
            new LevelConfig(15, "MoonTrucker.GameWorld.Map.txt"),
            new LevelConfig(15, "MoonTrucker.GameWorld.Level.txt")
        };
        private Level[] _levels;
        private int _currentLevel = 0;
        private bool _completedAllLevels = false;
        private World _world;
        private int _totalScore = 0;

        public int TotalScore => _totalScore;

        public GameLevels(float tileWidth, PropFactory propFactory, World world)
        {
            _world = world;
            _levels = new Level[_levelsConfig.Length];
            for (int i = 0; i < _levels.Length; i++)
            {
                _levels[i] = new Level(_levelsConfig[i], tileWidth, propFactory);
            }
        }

        public Level CurrentLevel => _levels[_currentLevel];

        public bool AllLevelsComplete => _completedAllLevels;

        public int CurrentLevelNumber => _currentLevel +1;

        public Level RestLevels()
        {
            _completedAllLevels = false;
            _currentLevel = 0;
            _totalScore = 0;
            return loadLevel();
        }

        public Level LoadNextLevel()
        {
            _totalScore += CurrentLevel.GetScore();

            _currentLevel++;
            if(_currentLevel >= _levels.Length)
            {
                _completedAllLevels = true;
                _currentLevel--; // Don't let the index get out of bounds
            }

            return loadLevel();
        }

        private Level loadLevel()
        {
            _world.Clear();
            CurrentLevel.Load();

            return CurrentLevel;
        }
    }
}
