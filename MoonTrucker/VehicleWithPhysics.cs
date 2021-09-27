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
        private const float IMPULSE_FACTOR = 1.5f;
        private const float MAX_SPEED = 70f;
        private const float TRACT_FACT = 1f;
        private const float TURN_FACTOR = 15f;
        private const float MAX_TRACTION_FORCE = 3f;
        private float _angle = 0;

        private Texture2D _sprite { get; }
        private Texture2D _light { get; }

        private Body _vehicleBody { get; }
        private SpriteBatch _batch;
        private bool _isBraking = false;
        private bool _inDrive = false;

        public float Height { get; }
        public float Width { get; }

        public VehicleWithPhysics(float width, float height, Vector2 position, World world, TextureManager manager, SpriteBatch batch, GraphicsDevice graphicsDevice)
        {

            Height = height;
            Width = width;

            _vehicleBody = BodyFactory.CreateRectangle(world, height, width, 1f, position, _angle, BodyType.Dynamic);

            //from https://box2d.org/documentation/md__d_1__git_hub_box2d_docs_dynamics.html
            _vehicleBody.LinearDamping = 0f; //makes car appear "floaty"
            _vehicleBody.AngularDamping = .01f;


            _vehicleBody.Restitution = 0.3f; //how bouncy (not bouncy)0-1(super bouncy) 
            _vehicleBody.Friction = 0.5f;    //friction between other bodies (none) 0 - 1 (frictiony)
            _vehicleBody.Inertia = 25f;
            _vehicleBody.Mass = 1f;

            _sprite = manager.TextureFromShape(_vehicleBody.FixtureList[0].Shape, Color.Transparent, Color.Salmon);
            _light = new Texture2D(graphicsDevice, 3, (int)ConvertUnits.ToDisplayUnits(width));
            Color[] colors = new Color[(3 * (int)ConvertUnits.ToDisplayUnits(width))];
            for (int i = 0; i < (3 * (int)ConvertUnits.ToDisplayUnits(width)); i++)
            {
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
            return Vector2.Dot(forwardVector, velVector) > 0;
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

        private void drawTailLights(Vector2 carOrigin)
        {
            Color tailLightColor;
            if (_vehicleBody.LinearVelocity.Length() == 0f)//stopped
            {
                tailLightColor = Color.DarkRed;
            }
            else //in motion
            {
                if (_inDrive)
                {
                    tailLightColor = Color.DarkRed;
                }

                else if (_isBraking)
                {
                    tailLightColor = Color.Red;
                }
                else
                {
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
            this.applyTraction();

        }

        private void snapVelocityToZero()
        {
            if (_vehicleBody.LinearVelocity.Length() < .7f)
            {
                _vehicleBody.LinearVelocity = Vector2.Zero;
                _vehicleBody.AngularVelocity = 0f;
            }
        }

        private void applyFriction()
        {
            _vehicleBody.ApplyAngularImpulse(-.05f * _vehicleBody.AngularVelocity * _vehicleBody.Inertia);
            _vehicleBody.ApplyLinearImpulse(GetDirectionalFrictionForce());

        }

        private void applyTraction()
        {
            if (_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var lateralVelocity = GetLateralVelocity();
                if (lateralVelocity.Length() > MAX_TRACTION_FORCE)
                {
                    lateralVelocity.Normalize();
                    lateralVelocity *= MAX_TRACTION_FORCE;
                }
                _vehicleBody.ApplyLinearImpulse(-lateralVelocity * _vehicleBody.Mass);
            }
        }

        private Vector2 GetForwardVelocity()
        {
            Vector2 forwardNormal = _vehicleBody.GetWorldVector(new Vector2(1, 0));
            return Vector2.Dot(forwardNormal, _vehicleBody.LinearVelocity) * forwardNormal;
        }
        private Vector2 GetLateralVelocity()
        {
            Vector2 rightNormal = _vehicleBody.GetWorldVector(new Vector2(0, -1));
            return Vector2.Dot(rightNormal, _vehicleBody.LinearVelocity) * rightNormal;
        }

        private Vector2 GetDirectionalFrictionForce()
        {
            Vector2 directionalNormal;
            float frictionMultiplier;
            if (VectorHelpers.IsMovingForward(_vehicleBody))
            {
                frictionMultiplier = -.01f;
                directionalNormal = _vehicleBody.GetWorldVector(new Vector2(1, 0));
            }
            else
            {
                frictionMultiplier = -.02f;
                directionalNormal = _vehicleBody.GetWorldVector(new Vector2(-1, 0));
            }
            return Vector2.Dot(directionalNormal, _vehicleBody.LinearVelocity) * directionalNormal * frictionMultiplier;
        }

        private void handleLeftKey()
        {
            if (_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var posNeg = this.isMovingForward() ? -1 : 1;
                float barelyMovingMultiplier = 1f;
                if (_vehicleBody.LinearVelocity.Length() < 10f) { barelyMovingMultiplier = .01f; }
                _vehicleBody.ApplyTorque(posNeg * _vehicleBody.Inertia * TURN_FACTOR * barelyMovingMultiplier);
            }
        }

        private void handleRightKey()
        {
            if (_vehicleBody.LinearVelocity.Length() != 0f)
            {
                var posNeg = this.isMovingForward() ? 1 : -1;
                float barelyMovingMultiplier = 1f;
                if (_vehicleBody.LinearVelocity.Length() < 10f) { barelyMovingMultiplier = .01f; }
                _vehicleBody.ApplyTorque(posNeg * _vehicleBody.Inertia * TURN_FACTOR * barelyMovingMultiplier);
            }
        }

        private void handleUpKey()
        {
            _inDrive = true;
            Vector2 impulse;
            if (_vehicleBody.LinearVelocity.Length() == 0f || (this.isMovingForward()))//stopped or accelerating
            {
                if (_vehicleBody.LinearVelocity.Length() > MAX_SPEED) { return; }
                impulse = this.getUnitDirectionVector();
            }
            else//decelerate
            {
                _isBraking = true;
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            _vehicleBody.ApplyLinearImpulse(impulse * _vehicleBody.Mass * IMPULSE_FACTOR);
        }

        private void handleDownKey()
        {
            _inDrive = false;
            Vector2 impulse;
            if (_vehicleBody.LinearVelocity.Length() == 0f || !this.isMovingForward())//stopped
            {
                if (_vehicleBody.LinearVelocity.Length() > MAX_SPEED) { return; }
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
            _vehicleBody.ApplyLinearImpulse(impulse * _vehicleBody.Mass * IMPULSE_FACTOR);
        }

        private Vector2 copyVector(Vector2 vector)
        {
            float vx, vy;
            vector.Deconstruct(out vx, out vy);
            return new Vector2(vx, vy);
        }

        private Vector2 getUnitDirectionVector()
        {
            return new Vector2(MathF.Cos(_vehicleBody.Rotation), MathF.Sin(_vehicleBody.Rotation));
        }
    }
}
