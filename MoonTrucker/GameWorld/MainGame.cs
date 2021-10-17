using System;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonTrucker.Core;
using MoonTrucker.Vehicle;

namespace MoonTrucker.GameWorld
{
    public class MainGame
    {
        private const int TOTAL_GAME_TIME = 15;

        private GameTarget _target;
        private SimpleVehicle _vehicle;
        private GameMap _map;
        private Timer _timer;
        private readonly World _world;
        private PropFactory _propFactory;
        private Camera2D _camera;
        private SpriteBatch _spriteBatch;

        public MainGame(Vector2 screenCenter, TextureManager manager, SpriteBatch spriteBatch, ResolutionIndependentRenderer renderer)
        {
            _spriteBatch = spriteBatch;
            _world = new World(new Vector2(0, 0)); //Create a phyics world with no gravity

            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 14 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(14f);

            _camera = new Camera2D(renderer);
            _camera.Zoom = 1f;
            _camera.RecalculateTransformationMatrices(); // TODO: This might not be needed

            _propFactory = new PropFactory(_world, manager, spriteBatch);

            _timer = new Timer(TimeSpan.FromSeconds(TOTAL_GAME_TIME));
            _vehicle = new SimpleVehicle(2f, 5f, screenCenter, _world, manager, spriteBatch);
            _camera.Position = _vehicle.GetPosition();
            _map = generateMap();

            _target = new GameTarget(_vehicle.Width, _map.GetRandomTargetLocation(), _propFactory);
            _map.Subscribe(_target);
            _timer.Subscribe(_target);
        }

        private GameMap generateMap()
        {
            var tileWidth = _vehicle.Height * 2f;
            return new GameMap(tileWidth, _propFactory, new Vector2(0, 0));
        }

        public float GetTimeLeft()
        {
            return (int)_timer.GetTime().TotalSeconds;
        }

        public int GetScore()
        {
            return _target.HitTotal;
        }

        public float GetAngleFromVehicleToTarget()
        {
            var targetPosition = _target.GetPosition();
            var vehiclePosition = _vehicle.GetPosition();
            var direction = new Vector2(targetPosition.X - vehiclePosition.X, targetPosition.Y - vehiclePosition.Y);
            direction.Normalize();
            float angle;

            // TODO: Don't rely on catch the math error. Just handle that case;
            try
            {
                angle = MathF.Atan(direction.Y / direction.X);
            }
            catch
            {
                angle = direction.Y > 0 ? (MathF.PI * 3f) / 2f : MathF.PI / 2;
            }
            if (targetPosition.X < vehiclePosition.X)
            {
                angle += MathF.PI;
            }

            return angle;
        }

        public void RestartGame()
        {
            _timer.AddTime(TimeSpan.FromSeconds(TOTAL_GAME_TIME));
            _target.SetPosition(_map.GetRandomTargetLocation());
            _target.ResetHitTotal();
        }

        public bool IsGameOver()
        {
            return _timer.IsTimerUp();
        }

        public void Update(GameTime gameTime, KeyboardState newKeyboardState)
        {
            _vehicle.UpdateVehicle(newKeyboardState, gameTime);
            _timer.Update(gameTime);

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            _camera.Position = ConvertUnits.ToDisplayUnits(_vehicle.GetPosition());
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
