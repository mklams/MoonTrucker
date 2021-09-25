using Genbox.VelcroPhysics.Collision.Shapes;
using Genbox.VelcroPhysics.Definitions;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker
{
    public class SimpleTire
    {
        private Body _body;
        private World _world;
        public SimpleTire(World world)
        {
            _world = world;

            BodyDef bodyDef = new BodyDef();
            bodyDef.Type = BodyType.Dynamic;
            _body = world.CreateBody(bodyDef);

            PolygonShape polygonShape = new PolygonShape(1f);
            polygonShape.SetAsBox(.2f, 1f);
            _body.CreateFixture(polygonShape);

            _body.UserData = this;

        }

        public void OnDestroy()
        {
            _world.DestroyBody(_body);
        }

        public Vector2 GetLateralVelocity()
        {
            Vector2 rightNormal = _body.GetWorldVector(new Vector2(1, 0));
            return Vector2.Dot(rightNormal, _body.LinearVelocity) * rightNormal;
        }

        public Vector2 GetForwardVelocity()
        {
            Vector2 forwardNormal = _body.GetWorldVector(new Vector2(0, 1));
            return Vector2.Dot(forwardNormal, _body.LinearVelocity) * forwardNormal;
        }

        private void updateFriction()
        {
            Vector2 impulse = _body.Mass * -GetLateralVelocity();
            _body.ApplyLinearImpulse(impulse, _body.WorldCenter);
            _body.ApplyAngularImpulse(0.1f * _body.Inertia * _body.AngularVelocity);

            Vector2 forwardVelocity = GetForwardVelocity();
            float forwardSpeed = forwardVelocity.Length();
            float dragMagnitude = forwardSpeed * -.2f;
            _body.ApplyLinearImpulse(dragMagnitude * forwardVelocity, _body.WorldCenter);

        }


    }
}