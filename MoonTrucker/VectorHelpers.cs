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
}