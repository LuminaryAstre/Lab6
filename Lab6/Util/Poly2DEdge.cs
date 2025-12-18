using Microsoft.Xna.Framework;

namespace Lab6.Util;

public struct Poly2DEdge(Vector2 a, Vector2 b)
{
    public Vector2 A = a;
    public Vector2 B = b;

    public Poly2DEdge Extend()
    {
        var dir = (A - B).ClampLength(1, 1);
        return new Poly2DEdge(A + dir * 5000, B - dir * 5000);
    }
}