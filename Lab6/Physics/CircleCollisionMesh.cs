using System.Collections.Generic;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Lab6.Physics;

public class CircleCollisionMesh : CollisionMesh
{
    public CircleF Shape;
    public CircleCollisionMesh(float radius, int? polygonResolution = null) : base(CreatePolygon(radius, polygonResolution ?? CollisionComplexityUtil.Circle))
    {
        Shape = new CircleF(Vector2.One * radius, radius);
    }

    public new CircleF GetTransformedShape()
    {
        return new CircleF(Position, Shape.Radius);
    }

    public override Vector2 DetectCollidePos(ICollisionMesh other)
    {
        if (other is CircleCollisionMesh otherC)
        {
            var otherCircle = otherC.GetTransformedShape();
            var self = GetTransformedShape();
            return otherCircle.Intersects(self) ? otherCircle.ClosestPointTo(self.Center) : new Vector2(float.NaN, float.NaN);
        }
        else
        {
            return base.DetectCollidePos(other);
        }
    }

    public override bool DetectCollide(ICollisionMesh other)
    {
        if (other is CircleCollisionMesh otherC)
        {
            return otherC.GetTransformedShape().Intersects(GetTransformedShape());
        }
        else
        {
            return base.DetectCollide(other);
        }
    }

    public override void Draw(SpriteBatch sprite)
    {
        var sh = GetTransformedShape();
        sprite.DrawRectangle(sh.BoundingRectangle.Moved(Position-sh.BoundingRectangle.Center), Color.Crimson);
        sprite.DrawCircle(sh, 32, Color.Green);
    }

    public static Polygon CreatePolygon(float radius, int res)
    {
        List<Vector2> segments = [];
        for (int i = 0; i < res; i++)
        {
            Vector2 v = Vector2.UnitY * radius;
            v.Rotate((360 * i / (float)res).ToRadians());
            segments.Add(v);
        }

        return new Polygon(segments);
    }
}