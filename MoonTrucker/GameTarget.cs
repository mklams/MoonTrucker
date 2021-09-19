using System;
using Microsoft.Xna.Framework;

namespace MoonTrucker
{
    public class GameTarget
    {
        public readonly CircleProp Body;

        public GameTarget(float radius, Vector2 position, PropFactory bodyFactory)
        {
            Body = bodyFactory.CreateCircleSensor(radius, position);

        }

        public void Draw()
        {
            Body.Draw();
        }
    }
}
