using System;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace MoonTrucker{

    public enum Direction{
        Forwards,
        Backwards
    }
    public class Tire{

        private const float MAX_FORWARD_DRIVE_FORCE = 2f;
        private const float MAX_BACKWARD_DRIVE_FORCE = 1f;
        private const float MAX_BRAKE_FORCE = 3f;
        private const float MAX_FRICTION_FORCE = 1f;
        private const float MAX_ROTATION_ANGLE_RADS = MathF.PI/4;
        private const float FRICTION_FORCE = .2f;
        
        private Vector2 _offset;
        private float _rotation = 0;
        private bool _turningWheel;
        private bool _driveWheel;
        public Tire(Vector2 offset, bool canTurn, bool canDrive)
        {
            _turningWheel = canTurn;
            _driveWheel = canDrive;
            _offset = offset;
        }

        public void Turn(float radians)
        {
            if(!_turningWheel){ return; }
            var newRotation = _rotation + radians;
            if(MathF.Abs(newRotation) < MAX_ROTATION_ANGLE_RADS)
            {
                _rotation = newRotation;
            }
        }

        public void ApplyForwardDriveForce(Body vehicleBody)
        {
            if(!_driveWheel) {return;}
            Vector2 tirePosition = this.getTirePosition(vehicleBody);
            Vector2 impulse = new Vector2(MAX_FORWARD_DRIVE_FORCE, 0);
            impulse = VectorHelpers.Rotate(impulse, vehicleBody.Rotation);
            if(_turningWheel)
            {
                impulse = VectorHelpers.Rotate(impulse, _rotation);
            }
            vehicleBody.ApplyLinearImpulse(impulse, tirePosition);
        }

        public void ApplyReverseDriveForce(Body vehicleBody)
        {
            if(!_driveWheel){return;}
            Vector2 tirePosition = this.getTirePosition(vehicleBody);
            Vector2 impulse = new Vector2(MAX_BACKWARD_DRIVE_FORCE, 0);
            impulse = VectorHelpers.Rotate(impulse, vehicleBody.Rotation);
            if(_turningWheel)
            {
                impulse = VectorHelpers.Rotate(impulse, _rotation);
            }
            vehicleBody.ApplyLinearImpulse(-impulse, tirePosition);
        }

        public void ApplyBrakeForce(Body vehicleBody)
        {
            if(VectorHelpers.IsStopped(vehicleBody)) {return;}
            if(VectorHelpers.IsNearStop(vehicleBody))
            {
                vehicleBody.ResetDynamics();
                return;
            }
            this.applyDragForce(vehicleBody, MAX_BRAKE_FORCE);
        }

        private void applyDragForce(Body vehicleBody, float force)
        {
            Vector2 tirePosition = this.getTirePosition(vehicleBody);
            Vector2 impulse = new Vector2(force, 0);
            impulse = VectorHelpers.Rotate(impulse, vehicleBody.Rotation);
            if(_turningWheel)
            {
                impulse = VectorHelpers.Rotate(impulse, _rotation);
            }
            if(VectorHelpers.IsMovingForward(vehicleBody))
            {
                impulse = -impulse;
            }
            vehicleBody.ApplyLinearImpulse(impulse, tirePosition);
        }

        public void ApplyFrictionForce(Body vehicleBody)
        {
            if(VectorHelpers.IsStopped(vehicleBody)) {return;}
            this.applyDragForce(vehicleBody, FRICTION_FORCE);
        }

        public void ApplyTractionForce(Body vehicleBody)
        {
            if(VectorHelpers.IsStopped(vehicleBody)){return;}
            Vector2 tirePosition = this.getTirePosition(vehicleBody);
            Vector2 unitTireDirec = new Vector2(1, 0);
            unitTireDirec = VectorHelpers.Rotate(unitTireDirec, vehicleBody.Rotation);
            if(_turningWheel)
            {
                unitTireDirec = VectorHelpers.Rotate(unitTireDirec, _rotation);
            }
            var angleBetween = VectorHelpers.AngleBetween(VectorHelpers.GetUnitDirectionVector(vehicleBody),vehicleBody.LinearVelocity);
            var complement = (MathF.PI/2f)-angleBetween;
            var magnitude = vehicleBody.LinearVelocity.Length()*MathF.Cos(complement);
            var impulse = VectorHelpers.Rotate(unitTireDirec, -MathF.PI/2f)*magnitude;
            if(VectorHelpers.IsTurningLeft(vehicleBody))
            {
                impulse = -impulse;
            }
            vehicleBody.ApplyLinearImpulse(impulse);
        }

        private Vector2 getTirePosition(Body vehicleBody)
        {
            return vehicleBody.Position + VectorHelpers.Rotate(_offset, vehicleBody.Rotation);
        }
    }
}