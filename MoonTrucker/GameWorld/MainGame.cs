using System;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
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

        private GameLevels _levels;
        private GameTarget _target;
        private SimpleVehicle _vehicle;
        private GameMap _map;
        private Timer _timer;
        private readonly World _world;
        private PropFactory _propFactory;
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;
        private TextureManager _manager;

        private Song _gameMusic;

        public MainGame(TextureManager manager, SpriteBatch spriteBatch, ResolutionIndependentRenderer renderer, Song gameMusic)
        {
            // TODO: Inject levels
            var levels = new Level[3]
            {
                new Level(15, 1),
                new Level(15, 2),
                new Level(15, 3)
            };
            _levels = new GameLevels(levels);

            _spriteBatch = spriteBatch;
            _gameMusic = gameMusic;
            _manager = manager;
            _world = new World(new Vector2(0, 0)); //Create a phyics world with no gravity

            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 14 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(14f);
            createCamera(renderer);

            _propFactory = new PropFactory(_world, manager, spriteBatch);

            _timer = new Timer(_levels.CurrentLevelTimeLimit);
            _map = generateMap();
            _vehicle = new SimpleVehicle(V_WIDTH, V_HEIGHT, _map.GetStartPosition(), _world, manager, spriteBatch);
            _camera.Position = _vehicle.GetPosition();
            _target = new GameTarget(_vehicle.Width, _map.GetRandomTargetLocation(), _propFactory);
            _map.Subscribe(_target);
            _timer.Subscribe(_target);
        }

        private void createCamera(ResolutionIndependentRenderer renderer)
        {
            _camera = new Camera2D(renderer);
            _camera.Zoom = 1f;
            _camera.RecalculateTransformationMatrices(); // TODO: This might not be needed
        }

        private GameMap generateMap()
        {
            var tileWidth = V_HEIGHT * 2f;
            return new GameMap(tileWidth, _propFactory);
        }

        public float GetTimeLeft()
        {
            return (int)_timer.GetTime().TotalSeconds;
        }

        public int GetCurrentLevel()
        {
            return _levels.CurrentLevelNumber;
        }

        public int GetScore()
        {
            return _target.HitTotal;
        }

        public bool PlayerWon => _levels.AllLevelsComplete;

        public float GetAngleFromVehicleToFinish()
        {
            var finishPosition = _map.GetFinishPosition();
            var vehiclePosition = _vehicle.GetPosition();
            return VectorHelpers.GetAngleFromAToB(vehiclePosition, finishPosition);
        }

        public float GetAngleFromVehicleToTarget()
        {
            var targetPosition = _target.GetPosition();
            var vehiclePosition = _vehicle.GetPosition();
            return VectorHelpers.GetAngleFromAToB(vehiclePosition, targetPosition);
        }

        public void StartGame()
        {
            MediaPlayer.Stop();
            MediaPlayer.Play(_gameMusic);
            MediaPlayer.IsRepeating = true;
            _levels.RestLevels();
            setupLevel();
            _target.ResetHitTotal();
            _map.ResetMap();
        }

        private void setupLevel()
        {
            _timer.SetTime(_levels.CurrentLevelTimeLimit);
            _target.SetPosition(_map.GetRandomTargetLocation());
            _map.ResetMap();
            resetVehicle();
            _camera.SetPosition(_vehicle.GetPosition());
        }

        private void resetVehicle()
        {
            _vehicle.Destroy();
            _vehicle = new SimpleVehicle(V_WIDTH, V_HEIGHT, _map.GetStartPosition(), _world, _manager, _spriteBatch);
        }

        public bool IsGameOver()
        {
            return _timer.IsTimerUp() || _levels.AllLevelsComplete;
        }

        public void Update(GameTime gameTime, KeyboardState newKeyboardState)
        {
            updateLevel();

            _vehicle.UpdateVehicle(newKeyboardState, gameTime);
            _timer.Update(gameTime);

            //Check if finish should be activiated
            updateFinish();

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            _camera.Position = ConvertUnits.ToDisplayUnits(_vehicle.GetPosition());
        }

        public void updateLevel()
        {
            if (_map.IsPlayerInWinZone())
            {
                _levels.NextLevel();

                if (!_levels.AllLevelsComplete)
                {
                    setupLevel();
                }
            }
        }

        public bool LevelComplete => _target.HitTotal >= _levels.CurrentLevel.NumberOfTargets;

        private void updateFinish()
        {
            if(LevelComplete)
            {
                _map.ActivateFinish();
            }
        }

        public void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone,
              null, _camera.GetViewTransformationMatrix());
            _map.Draw();
            _target.Draw();
            _vehicle.Draw();
            _spriteBatch.End();
        }
    }
}
