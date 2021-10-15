using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;
using MoonTrucker.Vehicle;

namespace MoonTrucker.GameWorld
{
    public class HUD
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private GameTarget _target;
        private Timer _timer;
        private Texture2D _arrow;
        private float _screenWidthPx;
        private float _screenHeightPx;
        private ResolutionIndependentRenderer _independentRenderer;
        private SimpleVehicle _vehicle;

        public HUD(SpriteBatch spriteBatch, SpriteFont font, GameTarget target, Timer timer, TextureManager textureManager, float screenWidthPx, float screenHeightPx, ResolutionIndependentRenderer renderer, SimpleVehicle vehicle, ContentManager content )
        {
            _spriteBatch = spriteBatch;
            _font = font;
            _target = target;
            _timer = timer;
            //_arrow = textureManager.GetTexture("Arrow");
            _arrow = content.Load<Texture2D>("GameAssets/Arrow");
            _screenWidthPx = screenWidthPx;
            _screenHeightPx = screenHeightPx;
            _independentRenderer = renderer;
            _vehicle = vehicle;
        }


        public void Draw(GameState state)
        {
            drawScore();
            drawTimer();
            drawArrow();
            if(state == GameState.GameOver)
            {
                drawGameOver();
            }
        }

        private void drawScore()
        {
            var scorePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(5, 0));
            _spriteBatch.DrawString(_font, $"Score: {_target.HitTotal}", scorePosition, Color.Red);
        }

        private void drawTimer()
        {
            var timeLeft = ((int)_timer.GetTime().TotalSeconds);
            var timePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(200, 0));

            _spriteBatch.DrawString(_font, $"Countdown: {timeLeft}", timePosition, Color.Red);
        }

        private void drawGameOver()
        {
            //var messagePosition = _independentRenderer.ScaleMouseToScreenCoordinates(new Vector2(_screenWidthPx / 2f, _screenHeightPx / 2f));
            // TODO: Figure out length of text and use that to get width pos instead of magic number of 0.4f
            var messagePosition = new Vector2(_screenWidthPx * 0.4f, _screenHeightPx * (1 / 3f));

            _spriteBatch.DrawString(_font, "Game Over", messagePosition, Color.Red);
        }
        private void drawArrow()
        {
            var targetPosition = _target.GetPosition();
            var vehiclePosition = _vehicle.GetPosition();
            var direction = new Vector2(targetPosition.X - vehiclePosition.X, targetPosition.Y - vehiclePosition.Y);
            direction.Normalize();
            float angle;
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

            var arrowPosition = new Vector2(_screenWidthPx / 2f, 70);
            var arrowCenter = new Vector2(_arrow.Width / 2f, _arrow.Height / 2f);
            _spriteBatch.Draw(_arrow, arrowPosition, null, Color.White, angle, arrowCenter, new Vector2(.25f, .25f), SpriteEffects.None, 1f);
        }
    }
}
