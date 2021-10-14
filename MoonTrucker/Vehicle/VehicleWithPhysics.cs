using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using MoonTrucker.Core;

namespace MoonTrucker.Vehicle
{
    public class VehicleWithPhysics
    {
        private const float MAX_TORQUE = 10f;
        private const float MAX_SPEED = 60f;
        private const float TURN_FACTOR = 15f;
        private const float MAX_TRACTION_FORCE = 3f;

        private const float MAX_BOOST_SPEED = 150f;
        private const float BOOST_FACTOR = 80f;
        private const double BOOST_COOLDOWN = .7; //sec
        private float _angle = 0;

        private Texture2D _sprite { get; }
        private Texture2D _light { get; }

        private Body _vehicleBody { get; }
        private SpriteBatch _batch;
        private bool _isBraking = false;
        private bool _inDrive = false;

        private double _lastBoost = -BOOST_COOLDOWN;

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


            _vehicleBody.Restitution = 0.3f; //how bouncy (not bouncy) 0 - 1(super bouncy) 
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

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                this.handleSpaceBar(gameTime);
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
        private Vector2 GetDirectionalVelocity()
        {
            Vector2 directionalNormal;
            if (VectorHelpers.IsMovingForward(_vehicleBody))
            {
                directionalNormal = _vehicleBody.GetWorldVector(new Vector2(1, 0));
            }
            else
            {
                directionalNormal = _vehicleBody.GetWorldVector(new Vector2(-1, 0));
            }
            return Vector2.Dot(directionalNormal, _vehicleBody.LinearVelocity) * directionalNormal;
        }

        private void handleSpaceBar(GameTime currGT)
        {
            if (!VectorHelpers.IsStopped(_vehicleBody) && VectorHelpers.IsMovingForward(_vehicleBody) && (currGT.TotalGameTime.TotalSeconds - _lastBoost) > BOOST_COOLDOWN)
            {
                if (GetForwardVelocity().Length() > MAX_BOOST_SPEED) { return; }
                _lastBoost = currGT.TotalGameTime.TotalSeconds;
                var forwardNormal = _vehicleBody.GetWorldVector(new Vector2(1, 0));
                _vehicleBody.ApplyLinearImpulse(forwardNormal * _vehicleBody.Mass * BOOST_FACTOR);
            }
        }

        private void handleLeftKey()
        {
            if (!VectorHelpers.IsStopped(_vehicleBody))
            {
                if (MathF.Abs(_vehicleBody.AngularVelocity) > MAX_TORQUE) { return; }
                var posNeg = this.isMovingForward() ? -1 : 1;
                float barelyMovingMultiplier = 1f;
                if (_vehicleBody.LinearVelocity.Length() < 10f) { barelyMovingMultiplier = .01f; }
                _vehicleBody.ApplyTorque(posNeg * _vehicleBody.Inertia * TURN_FACTOR * barelyMovingMultiplier);
            }
        }

        private void handleRightKey()
        {
            if (!VectorHelpers.IsStopped(_vehicleBody))
            {
                if (MathF.Abs(_vehicleBody.AngularVelocity) > MAX_TORQUE) { return; }
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
                if (GetDirectionalVelocity().Length() > MAX_SPEED) { return; }
                impulse = _vehicleBody.GetWorldVector(new Vector2(1, 0));
            }
            else//decelerate
            {
                _isBraking = true;
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            _vehicleBody.ApplyLinearImpulse(impulse * _vehicleBody.Mass * getImpulseFactor());
        }

        private void handleDownKey()
        {
            _inDrive = false;
            Vector2 impulse;
            if (_vehicleBody.LinearVelocity.Length() == 0f || !this.isMovingForward())//stopped
            {
                if (_vehicleBody.LinearVelocity.Length() > MAX_SPEED) { return; }
                impulse = _vehicleBody.GetWorldVector(new Vector2(-1, 0)); ;
            }
            else //decelerate
            {
                _isBraking = true;
                impulse = this.copyVector(_vehicleBody.LinearVelocity);
                impulse.Normalize();
                impulse *= -1;
            }
            _vehicleBody.ApplyLinearImpulse(impulse * _vehicleBody.Mass * getImpulseFactor());
        }

        private float getImpulseFactor()
        {
            var direction = GetDirectionalVelocity();
            var speed = direction.Length();
            if (!VectorHelpers.IsStopped(_vehicleBody) && !VectorHelpers.IsMovingForward(_vehicleBody))
            {
                return 0.8f;
            }
            float speedPercentile = (speed / MAX_SPEED) * 100;
            if (speedPercentile < 25f)
            {
                return 1.5f;
            }
            else if (speedPercentile < ((35f / 80f) * 100))
            {
                return 1.3f;
            }
            else if (speedPercentile < 50f)
            {
                return 1.2f;
            }
            else if (speedPercentile < 67.5f)
            {
                return 1.1f;
            }
            else if (speedPercentile < 75)
            {
                return 1f;
            }
            else
            {
                return .9f;
            }
        }

        private Vector2 copyVector(Vector2 vector)
        {
            float vx, vy;
            vector.Deconstruct(out vx, out vy);
            return new Vector2(vx, vy);
        }
    }
}
