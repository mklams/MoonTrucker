using System;
using System.Collections;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker.GameWorld
{
    public class Level: IDrawable, IObserver<GameTarget>
    {
        private readonly LevelConfig _config;
        private GameMap _map;
        private int _score;
        private int _targetsHit;
        private Timer _timer;
        private Vector2 _randomTargetPosition;

        public bool IsInfiniteLevel => _config.IsInfiniteLevel;
        public TimeSpan TimeLimit => TimeSpan.FromSeconds(_config.TimeLimit);
        public bool LevelIsFinished => _map.IsPlayerInWinZone();
        public int TimeLeftInSeconds => _timer.GetTimeInSeconds();
        public bool AllTargetsCollected => _targetsHit >= _map.GetNumberOfTargets() + _config.AddtionalTargets;

        public Level(LevelConfig config, float tileWidth, PropFactory propFactory)
        {
            _config = config;
            _map = new GameMap(config, tileWidth, propFactory);
            _score = 0;
            _targetsHit = 0;
            _timer = new Timer(TimeLimit);
            _randomTargetPosition = new Vector2();
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
            return _randomTargetPosition;
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
            if (AllTargetsCollected && !IsInfiniteLevel)
            {
                _map.ActivateFinish();
            }
        }

        public bool ShowArrow()
        {
            return _targetsHit >= _map.GetNumberOfTargets();
        }

        private void targetHit()
        {
            _targetsHit++;
            int elapsedTime = _timer.GetElapsedTime();
            elapsedTime = (elapsedTime <= 0) ? 1 : elapsedTime;
            double points = 50 + 50 * (1.0 / elapsedTime);
            _score += (int)points;
        }

        private bool _useRandomTarget => _targetsHit >= _map.GetNumberOfTargets() && !AllTargetsCollected;

        #region IObserver<GameTarget> implmentation
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(GameTarget target)
        {
            targetHit();
            target.Hide();

            if(_useRandomTarget)
            {
                target.Show();
                _randomTargetPosition = _map.GetRandomTargetLocation();
                target.SetPosition(_randomTargetPosition);
            }
            
        }
        #endregion
    }


    public class LevelConfig
    {
        public readonly int TimeLimit;
        public readonly string MapName;
        public readonly bool IsInfiniteLevel;
        public readonly int AddtionalTargets;

        public LevelConfig(int timeLimt, string mapName, int addtionalTargets = 0, bool infiniteLevel = false)
        {
            TimeLimit = timeLimt;
            MapName = mapName;
            IsInfiniteLevel = infiniteLevel;
            AddtionalTargets = addtionalTargets;
        }
    }

    public class GameLevels
    {
        private LevelConfig[] _levelsConfig = new LevelConfig[3]
        {
            new LevelConfig(15, "MoonTrucker.GameWorld.Level.txt", 1),
            new LevelConfig(15, "MoonTrucker.GameWorld.Map.txt"),
            new LevelConfig(15, "MoonTrucker.GameWorld.Level.txt", 2)
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
