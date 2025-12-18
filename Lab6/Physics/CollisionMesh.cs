using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Lab6.Physics;

// Basic collision mesh.
public class CollisionMesh(Polygon shape) : ICollisionMesh
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }

    public virtual Polygon GetShape()
    {
        return shape;
    }

    public virtual Polygon GetTransformedShape()
    {
        var self = shape.RotatedAround(shape.BoundingRectangle.Center, Rotation.ToRadians());
        self.Offset(Position - shape.BoundingRectangle.Center);
        return self;
    }

    public virtual Vector2[] DetectCollideIntersection(ICollisionMesh other)
    {
        return Poly2DColliderinator.SutherlandHodgman(GetTransformedShape(), other.GetTransformedShape());
    }

    public virtual Vector2 DetectCollidePos(ICollisionMesh other)
    {
        return GetTransformedShape().CollidesWith(other.GetTransformedShape());
    }

    public virtual bool DetectCollide(ICollisionMesh other)
    {
        if (StarshipSettings.CollisionType != "Normal") return DetectCollideCheap(other);
        return !DetectCollidePos(other).IsNaN();
    }

    public virtual bool DetectCollideCheap(ICollisionMesh other)
    {
        return GetTransformedShape().BoundingRectangle.Intersects(other.GetTransformedShape().BoundingRectangle);
    }

    public virtual void Draw(SpriteBatch sprite)
    {
        var rotated = shape.RotatedAround(shape.BoundingRectangle.Center, Rotation.ToRadians());
        sprite.DrawRectangle(rotated.BoundingRectangle.Moved(Position-rotated.BoundingRectangle.Center), Color.Crimson);
        sprite.DrawPolygon(Position - shape.BoundingRectangle.Center, rotated, Color.Green);
        foreach (var shapeVertex in rotated.Vertices)
        {
            sprite.DrawCircle(shapeVertex + Position - shape.BoundingRectangle.Center, 1, 3, Color.Azure);
        }
    }
}