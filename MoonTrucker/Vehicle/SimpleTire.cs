using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Definitions;
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
        private const float MAX_TRACTION_FORCE = 10f;
        private const float _width = 1f;
        private const float _height = .2f;
        private Body _body;
        private World _world;
        private TextureManager _textureManager;
        private SpriteBatch _batch;
        private Texture2D _sprite;

        public SimpleTire(Vector2 position, World world, TextureManager manager, SpriteBatch batch)
        {
            _body = BodyFactory.CreateRectangle(world, _width, _height, 1f, position, 0, BodyType.Dynamic);
            _body.LinearDamping = 0f; //makes car appear "floaty"
            _body.AngularDamping = .01f;
            _textureManager = manager;

            _body.Restitution = 0.0f; //how bouncy (not bouncy) 0 - 1(super bouncy) 
            _body.Friction = 0f;    //friction between other bodies (none) 0 - 1 (frictiony)
            _body.Inertia = 0f;
            //_body.Mass = .2f;

            _world = world;
            _sprite = manager.TextureFromShape(_body.FixtureList[0].Shape, Color.Red, Color.Red);
            _batch = batch;
            _body.UserData = this;

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
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_body.Position), null, Color.White, _body.Rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public void OnDestroy()
        {
            _world.DestroyBody(_body);
        }

        public void UpdateFriction()
        {
            //Traction
            Vector2 impulse = _body.Mass * -VectorHelpers.GetLateralVelocity(_body);
            if (impulse.Length() > MAX_TRACTION_FORCE)
            {
                impulse.Normalize();
                impulse = impulse * MAX_TRACTION_FORCE;
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