using System;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;

namespace MoonTrucker
{
    public abstract class Vehicle
    {
        private Texture2D _sprite { get; }
        private Texture2D _light  {get; }

        protected Body _vehicleBody { get; }
        private SpriteBatch _batch;
        protected bool _isBraking = false; 

        public float Height { get;  }
        public float Width { get;  }
        public Vehicle(float width, float height, Vector2 position, World world, TextureManager manager, SpriteBatch batch, GraphicsDevice graphicsDevice)
        {
            Height = height;
            Width = width;

            _vehicleBody = BodyFactory.CreateRectangle(world, height, width, 1f,position, 0f, BodyType.Dynamic);
            _vehicleBody.Restitution = 0.3f;
            _vehicleBody.Friction = 0.5f;

            _sprite = manager.TextureFromShape(_vehicleBody.FixtureList[0].Shape, Color.Transparent, Color.Salmon);
            _light = new Texture2D(graphicsDevice, 3, (int)ConvertUnits.ToDisplayUnits(width));
            Color[] colors = new Color[(3 * (int)ConvertUnits.ToDisplayUnits(width))];
            for(int i = 0; i < (3 * (int)ConvertUnits.ToDisplayUnits(width)); i++){
                colors[i] = Color.White;
            }
            _light.SetData(colors);
            _batch = batch;
        }

        public Vector2 GetPosition()
        {
            return _vehicleBody.Position;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_vehicleBody.Position), null, Color.White, _vehicleBody.Rotation, origin, 1f, SpriteEffects.None, 0f);
            this.drawTailLights(origin);
        }

        private void drawTailLights(Vector2 carOrigin){
            Color tailLightColor;
            if(VectorHelpers.IsStopped(_vehicleBody))
            {
                tailLightColor = Color.DarkRed;
            }
            else //in motion
            {
                if(_isBraking){
                    tailLightColor = Color.Red;
                }
                else if(VectorHelpers.IsMovingForward(_vehicleBody)){
                    tailLightColor = Color.DarkRed;
                }
                else{
                    tailLightColor = Color.White;
                }
            }
            _batch.Draw(_light, ConvertUnits.ToDisplayUnits(_vehicleBody.Position), null, tailLightColor, _vehicleBody.Rotation, carOrigin, 1f, SpriteEffects.None, 0f);

        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            _isBraking = false;
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                this.handleUpKey();
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                this.handleDownKey();
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                this.handleLeftKey();
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                this.handleRightKey();
            }
            this.snapVelocityToZero();
            this.applyFriction();
            //this.applyTraction();
        }

        protected abstract void handleUpKey();
        protected abstract void handleDownKey();
        protected abstract void handleLeftKey();
        protected abstract void handleRightKey();
        protected abstract void snapVelocityToZero();
        protected abstract void applyFriction();
        protected abstract void applyTraction();

    }
}
