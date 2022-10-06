using System;
using System.Collections;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

namespace MoonTrucker.GameWorld
{
    public class Level : IDrawable, IObserver<GameTarget>
    {
        private readonly LevelConfig _config;
        private GameMap _map;
        private int _score;
        private int _targetsHit;
        
        private Vector2 _randomTargetPosition;

        public bool IsEndlessLevel => _config.IsEndlessMap;
        public bool AllTargetsCollected => (_targetsHit >= _map.GetNumberOfTargets() + _config.EndlessTargetCount) && !IsEndlessLevel;

        public Level(LevelConfig config, float tileWidth, PropFactory propFactory, SpriteBatch spriteBatch, TextureManager texMan)
        {
            _config = config;
            _map = new GameMap(config, tileWidth, propFactory);
            _map.InitializeGraphics(spriteBatch, texMan);
            _score = 0;
            _targetsHit = 0;
           
            _randomTargetPosition = new Vector2();
        }

        public void Load()
        {
            _map.Load();
            _map.SubscribeToTargets(this);
        }

        public void SubscribeTimeToTargets(Timer timer)
        {
            _map.SubscribeToTargets(timer);
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

        public bool IsLevelFinished()
        {
            return _map.IsPlayerInWinZone() ||
                (!_map.HasFinish() && AllTargetsCollected);
        }

        public void Draw()
        {
            _map.Draw();
        }

        public void Update(GameTime gameTime)
        {
            updateFinish();
            _map.Update(gameTime);
        }

        private void updateFinish()
        {
            if (AllTargetsCollected)
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
            _score += 100; // targets worth 100 points
        }

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

            if (IsEndlessLevel)
            {
                target.Show();
                _randomTargetPosition = _map.GetRandomTargetLocation();
                target.SetPosition(_randomTargetPosition);
            }

        }
        #endregion
    }

    public class GameConfig
    {
        public readonly LevelConfig[] Levels;
        public readonly int TimeLimit;

        public GameConfig(LevelConfig[] levels, int timeLimit)
        {
            Levels = levels;
            TimeLimit = timeLimit;
        }
    }

    public class LevelConfig
    {
        public readonly string MapName;
        public readonly int EndlessTargetCount;
        public bool IsEndlessMap => EndlessTargetCount > 0;
        public readonly Color ObstacleColor;

        public LevelConfig(string mapName, Color obstacleColor, int endlessTargetCount = 0)
        {
            MapName = mapName;
            EndlessTargetCount = endlessTargetCount;
            ObstacleColor = obstacleColor;
        }
    }

    public class GameLevels
    {
        private GameConfig _gameConfig;
        private Level[] _levels;
        private int _currentLevel = 0;
        private bool _completedAllLevels = false;
        private World _world;
        private int _totalScore = 0;
        private Timer _timer;

        public TimeSpan TimeLimit => TimeSpan.FromSeconds(_gameConfig.TimeLimit);
        public int TimeLeftInSeconds => _timer.GetTimeInSeconds();
        public int TotalScore => _totalScore;

        public GameLevels(GameConfig config, float tileWidth, PropFactory propFactory, World world, SpriteBatch spriteBatch, TextureManager texMan)
        {
            _gameConfig = config;
            _timer = new Timer(TimeLimit);
            
            _world = world;
            _levels = new Level[_gameConfig.Levels.Length];
            for (int i = 0; i < _levels.Length; i++)
            {
                _levels[i] = new Level(_gameConfig.Levels[i], tileWidth, propFactory, spriteBatch, texMan);
            }
        }

        public Level CurrentLevel => _levels[_currentLevel];

        public bool AllLevelsComplete => _completedAllLevels;

        public int CurrentLevelNumber => _currentLevel + 1;

        public void Update(GameTime gameTime)
        {
            CurrentLevel.Update(gameTime);
            _timer.Update(gameTime);
        }

        public bool IsTimeUp()
        {
            return _timer.IsTimerUp();
        }

        public void RestLevels()
        {
            _completedAllLevels = false;
            _currentLevel = 0;
            _totalScore = 0;
            loadLevel();
        }

        public void LoadNextLevel()
        {
            _totalScore += CurrentLevel.GetScore();

            _currentLevel++;
            if (_currentLevel >= _levels.Length)
            {
                _completedAllLevels = true;
                _currentLevel--; // Don't let the index get out of bounds
            }

            loadLevel();
        }

        private void loadLevel()
        {
            _world.Clear();
            CurrentLevel.Load();

            // TODO: Don't assume only level is endless mode. Pass in the game mode
            if (_gameConfig.Levels.Length == 1)
            {
                // only endless mode supports adding time when a target is hit
                CurrentLevel.SubscribeTimeToTargets(_timer);
            }
        }

        private double getElapsedTime()
        {
            int elapsedTime = _timer.GetElapsedTime();
            elapsedTime = (elapsedTime <= 0) ? 1 : elapsedTime;
            return elapsedTime;
        }
    }
}
