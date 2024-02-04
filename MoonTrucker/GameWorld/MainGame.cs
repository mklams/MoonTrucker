using System;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
using Genbox.VelcroPhysics.Extensions.DebugView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MoonTrucker.Core;
using MoonTrucker.Vehicle;

namespace MoonTrucker.GameWorld
{
    public class MainGame
    {
        private const float V_WIDTH = 2f;
        private const float V_HEIGHT = 5f;
        private const float TILE_WIDTH = V_HEIGHT * 2f;

        private GameLevels _levels;
        private SimpleVehicle _vehicle;
        private readonly World _world;
        private PropFactory _propFactory;
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;
        private TextureManager _manager;
        private Level _currentLevel;
        private Song _gameMusic;
        private GameMode _mode = GameMode.Arcade;
        private DateTime _startTime = DateTime.Now;
        private DateTime _endTime = DateTime.MaxValue;

        public TimeSpan PlayTime => _endTime - _startTime;

        public GameMode Mode => _mode;

        public MainGame(TextureManager manager, SpriteBatch spriteBatch, ResolutionIndependentRenderer renderer, Song gameMusic)
        {
            _spriteBatch = spriteBatch;
            _gameMusic = gameMusic;
            _manager = manager;
            _world = new World(new Vector2(0, 0)); //Create a phyics world with no gravity

            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 14 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(14f);
            createCamera(renderer);

            _propFactory = new PropFactory(_world, manager, spriteBatch);
            var test = PlayTime.TotalMilliseconds;
        }

        public Vector2 CurrentVehiclePosition()
        {
            return _vehicle.GetPosition();
        }

        public void SetMode(GameMode mode)
        {
            _mode = mode;
        }

        private void createCamera(ResolutionIndependentRenderer renderer)
        {
            _camera = new Camera2D(renderer);
            _camera.Zoom = 1f;
            _camera.RecalculateTransformationMatrices(); // TODO: This might not be needed
        }

        public float GetTimeLeft()
        {
            return _currentLevel.TimeLeftInSeconds;
        }

        public int GetCurrentLevelNumber()
        {
            return _levels.CurrentLevelNumber;
        }

        public long GetScore()
        {
            // TODO: MainGame should not need to figure out how to calculate the current score
            return _mode == GameMode.Arcade ? (long)PlayTime.TotalSeconds : _levels.TotalScore + _currentLevel.GetScore();
        }

        public bool PlayerWon => _levels.AllLevelsComplete;

        public float GetAngleFromVehicleToDestination()
        {
            return _currentLevel.AllTargetsCollected ? GetAngleFromVehicleToFinish() : GetAngleFromVehicleToTarget();
        }

        private float GetAngleFromVehicleToFinish()
        {
            var finishPosition = _currentLevel.GetFinishPosition();
            var vehiclePosition = _vehicle.GetPosition();
            return VectorHelpers.GetAngleFromAToB(vehiclePosition, finishPosition);
        }

        private float GetAngleFromVehicleToTarget()
        {
            var targetPosition = _currentLevel.GetTargetPosition();
            var vehiclePosition = _vehicle.GetPosition();
            return VectorHelpers.GetAngleFromAToB(vehiclePosition, targetPosition);
        }

        public bool ShowArrow()
        {
            return _currentLevel.ShowArrow();
        }

        public void StartGame(LevelConfig[] config)
        {
            _levels = new GameLevels(config, Mode, TILE_WIDTH, _propFactory, _world, _spriteBatch, _manager);
            MediaPlayer.Stop();
            MediaPlayer.Play(_gameMusic);
            MediaPlayer.IsRepeating = true;
            _currentLevel = _levels.RestLevels();
            setupVehicle();
            _startTime = DateTime.Now;
        }

        private void setupVehicle()
        {
            _vehicle = new SimpleVehicle(V_WIDTH, V_HEIGHT, _currentLevel.GetStartPosition(), _world, _manager, _spriteBatch);
            _camera.SetPosition(_vehicle.GetPosition());
        }

        public bool IsGameOver()
        {
            return _currentLevel.IsTimeUp() || _levels.AllLevelsComplete;
        }

        public void Update(GameTime gameTime, KeyboardState newKeyboardState)
        {
            updateLevel(gameTime);
            _vehicle.UpdateVehicle(newKeyboardState, gameTime);
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            _camera.Position = ConvertUnits.ToDisplayUnits(_vehicle.GetPosition());

            if (_levels.AllLevelsComplete)
            {
                _endTime = DateTime.Now;
            }
        }

        public void updateLevel(GameTime gameTime)
        {
            _currentLevel.Update(gameTime);
            if (_currentLevel.IsLevelFinished() && !_levels.AllLevelsComplete)
            {
                _currentLevel = _levels.LoadNextLevel();
                setupVehicle();
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone,
              null, _camera.GetViewTransformationMatrix());
            _currentLevel.Draw();
            _vehicle.Draw();
            _spriteBatch.End();
        }
    }
}
