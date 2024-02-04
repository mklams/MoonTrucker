using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

namespace MoonTrucker.Vehicle
{
    public class SimpleTire
    {
        private const float MAX_TRACTION_FORCE = 5f;
        private const float _width = 1f;
        private const float _height = .2f;
        private Body _body;
        private World _world;
        private SpriteBatch _batch;
        private Texture2D _sprite;
        private FreeformParticleTrail _tireTrail;
        private bool _sliding;

        public SimpleTire(Vector2 position, World world, TextureManager manager, SpriteBatch batch, bool hasTrail)
        {
            _body = BodyFactory.CreateRectangle(world, _width, _height, 1f, position, 0, BodyType.Dynamic);
            _body.LinearDamping = 0f; //makes car appear "floaty"
            _body.AngularDamping = .01f;

            _body.Restitution = 0.0f; //how bouncy (not bouncy) 0 - 1(super bouncy) 
            _body.Friction = 0f;    //friction between other bodies (none) 0 - 1 (frictiony)
            _body.Inertia = 0f;
            //_body.Mass = .2f;

            _world = world;
            _sprite = manager.TextureFromShape(_body.FixtureList[0].Shape, Color.Red, Color.Red);
            _batch = batch;
            _body.UserData = this;
            if (hasTrail)
            {
                _tireTrail = new FreeformParticleTrail(5, Color.Red, _batch, manager);
            }
        }

        public Body GetBody()
        {
            return _body;
        }

        public float GetWidth()
        {
            return _width;
        }

        public float GetHeight()
        {
            return _height;
        }

        public void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _tireTrail?.Draw();
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_body.Position), null, Color.White, _body.Rotation, origin, 1f, SpriteEffects.None, .5f);
        }

        public void OnDestroy()
        {
            _world.DestroyBody(_body);
        }

        public void UpdateFriction()
        {
            _sliding = false;
            //Traction
            Vector2 impulse = _body.Mass * -VectorHelpers.GetLateralVelocity(_body);
            if (impulse.Length() > MAX_TRACTION_FORCE)
            {
                _sliding = true;
                //impulse.Normalize(); This wasn't doing anything anyways... For now this just applies the tire trail.
                //impulse = impulse * MAX_TRACTION_FORCE; We never hit 75f of lateral force. Any skidding was just box 2d inertia. 
            }
            _body.ApplyLinearImpulse(impulse * 1.5f);//This 1.5 multiplier makes the car feel much better. Not my proudest fix... ::shrug::

            //Rotations inertia loss
            _body.ApplyAngularImpulse(-0.1f * _body.AngularVelocity);

            //Linear friction loss
            Vector2 directionalVelocity = VectorHelpers.GetDirectionalVelocity(_body);
            float directionalSpeed = directionalVelocity.Length();
            float dragMagnitude = .02f;
            _body.ApplyLinearImpulse(dragMagnitude * directionalSpeed * -VectorHelpers.GetDirectionalNormal(_body));

        }

        public void Update()
        {
            _tireTrail?.Update();
        }

        public void LogTireTrail()
        {
            if (_sliding)
            {
                _tireTrail?.CreateNewParticle(ConvertUnits.ToDisplayUnits(_body.Position));
            }
        }

        public void applyForwardDriveForce(float magnitude)
        {
            _body.ApplyLinearImpulse(magnitude * VectorHelpers.GetForwardNormal(_body));
        }

        public void applyReverseDriveForce(float magnitude)
        {
            _body.ApplyLinearImpulse(magnitude * VectorHelpers.GetBackwardsNormal(_body));
        }

        public void ApplyTorque(float torque)
        {
            _body.ApplyTorque(torque);
        }
    }
}