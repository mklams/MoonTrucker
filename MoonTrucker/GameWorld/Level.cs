﻿using System;
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
        private Timer _timer;
        private Vector2 _randomTargetPosition;

        public bool IsEndlessLevel => _config.IsEndlessMap;
        public TimeSpan TimeLimit => TimeSpan.FromSeconds(_config.TimeLimit);
        public bool LevelIsFinished => _map.IsPlayerInWinZone();
        public int TimeLeftInSeconds => _timer.GetTimeInSeconds();
        public bool AllTargetsCollected => (_targetsHit >= _map.GetNumberOfTargets() + _config.EndlessTargetCount) && !IsEndlessLevel;

        public Level(LevelConfig config, float tileWidth, PropFactory propFactory, SpriteBatch spriteBatch, TextureManager texMan)
        {
            _config = config;
            _map = new GameMap(config, tileWidth, propFactory);
            _map.InitializeGraphics(spriteBatch, texMan);
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

        private double getElapsedTime()
        {
            int elapsedTime = _timer.GetElapsedTime();
            elapsedTime = (elapsedTime <= 0) ? 1 : elapsedTime;
            return elapsedTime;
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

    public class LevelConfig
    {
        public readonly int TimeLimit;
        public readonly string MapName;
        public readonly int EndlessTargetCount;
        public bool IsEndlessMap => EndlessTargetCount > 0;

        public LevelConfig(int timeLimt, string mapName, int endlessTargetCount = 0)
        {
            TimeLimit = timeLimt;
            MapName = mapName;
            EndlessTargetCount = endlessTargetCount;
        }
    }

    public class GameLevels
    {
        private LevelConfig[] _levelsConfig;
        private Level[] _levels;
        private int _currentLevel = 0;
        private bool _completedAllLevels = false;
        private World _world;
        private int _totalScore = 0;

        public int TotalScore => _totalScore;

        public GameLevels(LevelConfig[] config, float tileWidth, PropFactory propFactory, World world, SpriteBatch spriteBatch, TextureManager texMan)
        {
            _levelsConfig = config;
            _world = world;
            _levels = new Level[_levelsConfig.Length];
            for (int i = 0; i < _levels.Length; i++)
            {
                _levels[i] = new Level(_levelsConfig[i], tileWidth, propFactory, spriteBatch, texMan);
            }
        }

        public Level CurrentLevel => _levels[_currentLevel];

        public bool AllLevelsComplete => _completedAllLevels;

        public int CurrentLevelNumber => _currentLevel + 1;

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
            if (_currentLevel >= _levels.Length)
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
