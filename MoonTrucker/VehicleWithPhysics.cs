using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;

namespace MoonTrucker
{
    public class VehicleWithPhysics
    {
        private const float IMPULSE_FACTOR = 8f;
        private const float TRACT_FACT = .3f;
        private const float TURN_FACTOR = 100f;
        private float _angle = 0;

        private Texture2D _sprite { get; }
        private Texture2D _light  {get; }

        private Body _vehicleBody { get; }
        private SpriteBatch _batch;
        private bool _isBraking = false;

        public float Height { get;  }
        public float Width { get;  }

        public VehicleWithPhysics(float width, float height, Vector2 position, World world, TextureManager manager, SpriteBatch batch, GraphicsDevice graphicsDevice)
        {
            
            Height = height;
            Width = width;

            _vehicleBody = BodyFactory.CreateRectangle(world, height, width, 1f,position, _angle, BodyType.Dynamic);
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

        public bool isMovingForward()
        {
            var forwardVector = new Vector2(MathF.Cos(_vehicleBody.Rotation), MathF.Sin(_vehicleBody.Rotation));
            var velVector = this.copyVector(_vehicleBody.LinearVelocity);
            velVector.Normalize();
            return Vector2.Dot( forwardVector, velVector) > 0;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_vehicleBody.Position), null, Color.White, _vehicleBody.Rotation, origin, 1f, SpriteEffects.None, 0f);
            this.drawTailLights(origin);
        }

        public Vector2 GetPosition()
        {
            return _vehicleBody.Position;
        }

        private void drawTailLights(Vector2 carOrigin){
            Color tailLightColor;
            if(_vehicleBody.LinearVelocity.Length() == 0f)//stopped
            {
                tailLightColor = Color.DarkRed;
            }
            else //in motion
            {
                if(_isBraking){
                    tailLightColor = Color.Red;
                }
                else if(isMovingForward()){
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
            this.applyRotationalFriction();
            this.applyTraction();

        }

        private void snapVelocityToZero()
        {
            if(_vehicleBody.LinearVelocity.Length() < .1f){
                _vehicleBody.LinearVelocity = Vector2.Zero;
                _vehicleBody.AngularVelocity = 0f;
            }
        }

        private void applyRotationalFriction(){
            _vehicleBody.AngularVelocity *= .98f;
            //_vehicleBody.LinearVelocity *= .98f;
        }

        private void applyTraction()
        {
            if(_vehicleBody.LinearVelocity.Length() != 0f)
            {
                _vehicleBody.ApplyLinearImpulse(-_vehicleBody.LinearVelocity * TRACT_FACT);                
            }
        }

        private void handleLeftKey()
        {
            if(_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var posNeg = this.isMovingForward()? -1 : 1;
                _vehicleBody.ApplyTorque(posNeg * TURN_FACTOR);
            }
        }

        private void handleRightKey()
        {
            if(_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var posNeg = this.isMovingForward()? 1 : -1;
                _vehicleBody.ApplyTorque(posNeg * TURN_FACTOR);
            }
        }

        private void handleUpKey()
        {
            Vector2 impulse;
            if(_vehicleBody.LinearVelocity.Length() == 0f || (this.isMovingForward()))//stopped or accelerating
            {
                impulse = this.getUnitDirectionVector();
            }
            else//decelerate
            {
                _isBraking = true;
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            _vehicleBody.ApplyLinearImpulse(impulse*IMPULSE_FACTOR);
        }

        private void handleDownKey()
        {
            Vector2 impulse;
            if(_vehicleBody.LinearVelocity.Length() == 0f || !this.isMovingForward())//stopped
            {
                impulse = this.getUnitDirectionVector();
                impulse *= -1;
            }
            else //decelerate
            {
                _isBraking = true;
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            _vehicleBody.ApplyLinearImpulse(impulse*IMPULSE_FACTOR);
        }

        private Vector2 copyVector(Vector2 vector)
        {
            float vx,vy;
            vector.Deconstruct(out vx, out vy);
            return new Vector2(vx,vy);
        }

        private Vector2 getUnitDirectionVector()
        {
            return new Vector2(MathF.Cos(_vehicleBody.Rotation), MathF.Sin(_vehicleBody.Rotation));
        }
    }
}
