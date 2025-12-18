using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace Lab6.Physics;

public interface ICollisionMesh
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Polygon GetShape();
    public Polygon GetTransformedShape();
    public Vector2[] DetectCollideIntersection(ICollisionMesh other);
    public Vector2 DetectCollidePos(ICollisionMesh other);
    public bool DetectCollide(ICollisionMesh other);
    public bool DetectCollideCheap(ICollisionMesh other);
    public void Draw(SpriteBatch sprite);
}