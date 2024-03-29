using System;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Definitions.Joints;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonTrucker.Core;

namespace MoonTrucker.Vehicle
{
    enum Tires
    {
        FrontLeft = 0,
        FrontRight = 1,
        BackLeft = 2,
        BackRight = 3
    }
    public class SimpleVehicle : Vehicle
    {
        private List<SimpleTire> _tires; //[frontLeft,frontRight,backLeft,backRight]
        private RevoluteJoint frontLeftJoint, frontRightJoint;
        private double _lastBoost = -BOOST_COOLDOWN;
        private const float MAX_BOOST_SPEED = 150f;
        private const float BOOST_FACTOR = 80f;
        private const double BOOST_COOLDOWN = .7; //sec
        private const float TURN_FACTOR = .015f;
        private const float MAX_SPEED = 80f;
        private const float BRAKING_FORCE = 1f;

        private const float MAX_TURN_ANGLE = MathF.PI / 6f;//30 degrees in radians
        public SimpleVehicle(float width, float height, Vector2 position, World world, Color mainColor, Color contrastColor, TextureManager manager, SpriteBatch batch)
            : base(width, height, position, world, mainColor, contrastColor, manager, batch)
        {
            _tires = new List<SimpleTire>();
            initializeTires();
        }

        public Body GetBody()
        {
            return _body;
        }

        public override void Draw()
        {
            base.Draw();
            _tires.ForEach(tire => tire.Draw());

        }

        protected override void restorativeTurn(GameTime gameTime)
        {
            if (!_isTurning)
            {
                frontLeftJoint.SetLimits(0f, 0f);
                frontRightJoint.SetLimits(0f, 0f);
            }
        }

        protected override void applyFriction()
        {
            _tires.ForEach(tire => tire.UpdateFriction());
        }

        protected override void updateEffects()
        {
            _tires.ForEach((tire) =>
            {
                tire.Update();
                tire.LogTireTrail();
            });
        }

        protected override void handleDownKey(GameTime gameTime)
        {
            _inDrive = false;
            if (_body.LinearVelocity.Length() == 0f || (!VectorHelpers.IsMovingForward(_body)))//stopped or accelerating backwards
            {
                if (VectorHelpers.GetDirectionalVelocity(_body).Length() > MAX_SPEED) { return; }
                _tires.ForEach(tire => tire.applyReverseDriveForce(_body.Mass * .3f));
            }
            else//decelerate
            {
                _isBraking = true;
                _tires.ForEach(tire => tire.applyReverseDriveForce(_body.Mass * BRAKING_FORCE));
            }
        }

        protected override void handleLeftKey(GameTime gameTime)
        {
            if (frontLeftJoint.JointAngle > -MAX_TURN_ANGLE)
            {
                var newAngle = frontLeftJoint.JointAngle;
                if (newAngle > 0)
                {
                    newAngle = 0;
                }
                newAngle = newAngle - TURN_FACTOR;
                frontLeftJoint.SetLimits(newAngle, newAngle);
                frontRightJoint.SetLimits(newAngle, newAngle);
            }
        }

        protected override void handleRightKey(GameTime gameTime)
        {
            if (frontLeftJoint.JointAngle < MAX_TURN_ANGLE)
            {
                var newAngle = frontLeftJoint.JointAngle;
                if (newAngle < 0)
                {
                    newAngle = 0;
                }
                newAngle = newAngle + TURN_FACTOR;
                frontLeftJoint.SetLimits(newAngle, newAngle);
                frontRightJoint.SetLimits(newAngle, newAngle);
            }
        }

        protected override void handleUpKey(GameTime gameTime)
        {
            _inDrive = true;
            if (_body.LinearVelocity.Length() == 0f || (VectorHelpers.IsMovingForward(_body)))//stopped or accelerating
            {
                if (VectorHelpers.GetDirectionalVelocity(_body).Length() > MAX_SPEED) { return; }
                _tires.ForEach(tire => tire.applyForwardDriveForce(_body.Mass * .3f));
            }
            else//decelerate
            {
                _isBraking = true;
                _tires.ForEach(tire => tire.applyForwardDriveForce(_body.Mass * BRAKING_FORCE));
            }
        }

        protected override void handleSpaceBar(GameTime gameTime)
        {
            if (!VectorHelpers.IsStopped(_body) && VectorHelpers.IsMovingForward(_body) && (gameTime.TotalGameTime.TotalSeconds - _lastBoost) > BOOST_COOLDOWN)
            {
                if (VectorHelpers.GetForwardVelocity(_body).Length() > MAX_BOOST_SPEED) { return; }
                _lastBoost = gameTime.TotalGameTime.TotalSeconds;
                var forwardNormal = _body.GetWorldVector(new Vector2(1, 0));
                _body.ApplyLinearImpulse(forwardNormal * _body.Mass * BOOST_FACTOR);
            }
        }

        protected override void snapVelocityToZero()
        {
            if (_body.LinearVelocity.Length() > 0f && _body.LinearVelocity.Length() < 1f)
            {
                _body.LinearVelocity = Vector2.Zero;
                _body.AngularVelocity = 0f;
            }
        }

        private void initializeTires()
        {
            //Front left tire
            var jointDef = this.createJointDef();
            var relPosition = new Vector2(1.7f, -0.6f);
            var tire = new SimpleTire(_body.GetWorldPoint(relPosition), _world, _textureManager, _batch, false, null);
            jointDef.BodyB = tire.GetBody();
            jointDef.LocalAnchorA = relPosition;
            frontLeftJoint = (RevoluteJoint)_world.CreateJoint(jointDef);
            _tires.Add(tire);

            //Front right tire
            jointDef = this.createJointDef();
            relPosition = new Vector2(1.7f, 0.6f);
            tire = new SimpleTire(_body.GetWorldPoint(relPosition), _world, _textureManager, _batch, false, null);
            jointDef.BodyB = tire.GetBody();
            jointDef.LocalAnchorA = relPosition;
            frontRightJoint = (RevoluteJoint)_world.CreateJoint(jointDef);
            _tires.Add(tire);

            //Back left tire
            jointDef = this.createJointDef();
            relPosition = new Vector2(-1.7f, -0.6f);
            tire = new SimpleTire(_body.GetWorldPoint(relPosition), _world, _textureManager, _batch, true, _contrastColor);
            jointDef.BodyB = tire.GetBody();
            jointDef.LocalAnchorA = relPosition;
            _world.CreateJoint(jointDef);
            _tires.Add(tire);

            //Back right tire
            jointDef = this.createJointDef();
            relPosition = new Vector2(-1.7f, 0.6f);
            tire = new SimpleTire(_body.GetWorldPoint(relPosition), _world, _textureManager, _batch, true, _contrastColor);
            jointDef.BodyB = tire.GetBody();
            jointDef.LocalAnchorA = relPosition;
            _world.CreateJoint(jointDef);
            _tires.Add(tire);
        }

        private RevoluteJointDef createJointDef()
        {
            var jointDef = new RevoluteJointDef();
            jointDef.BodyA = _body;
            jointDef.EnableLimit = true;
            jointDef.LowerAngle = 0;
            jointDef.UpperAngle = 0;
            jointDef.LocalAnchorB = Vector2.Zero;
            return jointDef;
        }

        private float getImpulseFactor()
        {
            var direction = VectorHelpers.GetDirectionalVelocity(_body);
            var speed = direction.Length();
            if (!VectorHelpers.IsStopped(_body) && !VectorHelpers.IsMovingForward(_body))
            {
                return 0.1f;
            }
            float speedPercentile = (speed / MAX_SPEED) * 100;
            if (speedPercentile < 25f)
            {
                return .15f;
            }
            else if (speedPercentile < ((35f / 80f) * 100))
            {
                return .13f;
            }
            else if (speedPercentile < 50f)
            {
                return .12f;
            }
            else if (speedPercentile < 67.5f)
            {
                return .11f;
            }
            else if (speedPercentile < 75)
            {
                return .1f;
            }
            else
            {
                return .1f;
            }
        }
    }


}