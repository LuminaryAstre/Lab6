using Lab6.Physics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace Lab6.Util;

public static class CollisionComplexityUtil
{
    public static ICollisionMesh GetCollision(Vector2[] points)
    {
        if (StarshipSettings.CollisionType == "Normal") return new CollisionMesh(new Polygon(points));
        else
        {
            return new CircleCollisionMesh(new Polygon(points).BoundingRectangle.Width / 2f);
        }
    }

    public static int Circle
    {
        get
        {
            switch (StarshipSettings.CollisionType)
            {
                case "Normal": return 32;
                case "Cheap": return 16;
                case "Cheaper": return 8;
                default: return 6;
            }
        }
    }
}