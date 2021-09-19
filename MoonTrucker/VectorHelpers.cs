using System;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;

public static class VectorHelpers{
    public static Vector2 CopyVector(Vector2 vector)
    {
        float vx,vy;
        vector.Deconstruct(out vx, out vy);
        return new Vector2(vx,vy);
    }

    public static Vector2 GetUnitDirectionVector(Body vehicleBody)
    {
        return new Vector2(MathF.Cos(vehicleBody.Rotation), MathF.Sin(vehicleBody.Rotation));
    }

    public static bool IsMovingForward(Body vehicleBody)
    {
        var forwardVector = new Vector2(MathF.Cos(vehicleBody.Rotation), MathF.Sin(vehicleBody.Rotation));
        var velVector = VectorHelpers.CopyVector(vehicleBody.LinearVelocity);
        velVector.Normalize();
        return Vector2.Dot( forwardVector, velVector) > 0;
    }

    public static bool IsStopped(Body vehicleBody)
    {
        return vehicleBody.LinearVelocity.Length() == 0f;
    }

    public static Vector2 Rotate(Vector2 vector, float radians)
    {
        float sin = MathF.Sin(radians);
        float cos = MathF.Cos(radians);
        float tx = vector.X;
        float ty = vector.Y;
        return new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
    }

    public static bool IsTurningLeft(Body vehicleBody)
    {
        //https://stackoverflow.com/questions/13221873/determining-if-one-2d-vector-is-to-the-right-or-left-of-another
        var direction = VectorHelpers.GetUnitDirectionVector(vehicleBody);
        var velocity = VectorHelpers.CopyVector(vehicleBody.LinearVelocity);
        var dot = (direction.X*(-velocity.Y))+(direction.Y*velocity.X);
        return dot > 0;
    }

    public static float AngleBetween(Vector2 a, Vector2 b)
    {
        return MathF.Acos(Vector2.Dot(a,b)/(a.Length()*b.Length()));
    }

    public static bool IsNearStop(Body vehicleBody)
    {
        return (!VectorHelpers.IsStopped(vehicleBody) && vehicleBody.LinearVelocity.Length() < .4f);
    }
}