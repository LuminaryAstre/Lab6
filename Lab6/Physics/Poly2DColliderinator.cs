using System.Collections.Generic;
using System.Linq;
using Lab6.Util;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace Lab6.Physics;

// https://en.wikipedia.org/wiki/Sutherland%E2%80%93Hodgman_algorithm
public static class Poly2DColliderinator
{
    public static Vector2 ComputeIntersection(Vector2 p1, Vector2 p2, Poly2DEdge line)
    {
        Vector2 dc = p1 - p2;
        Vector2 dp = line.A - line.B;
        float n1 = p1.X * p2.Y - p1.Y * p2.X;
        float n2 = line.A.X * line.B.Y - line.A.Y * line.B.X;
        float n3 = 1.0f / (dc.X * dp.Y - dc.Y * dp.X);
        return new Vector2((n1 * dp.X - n2 * dc.X) * n3, (n1 * dp.Y - n2 * dc.Y) * n3);
    }
    public static Vector2[] SutherlandHodgman(Polygon self, Polygon other)
    {
        List<Vector2> outputList = self.Vertices.ToList();

        foreach (var clipEdge in other.GetEdges())
        {
            var inputList = outputList.Copy();
            outputList.Clear();

            for (int i = 0; i < inputList.Count; i++)
            {
                var currentPoint = inputList[i];
                // TODO: make a modulo function for int
                var prevPoint = inputList[(int)(i - 1f).Modulo(inputList.Count)];

                Vector2 intersectingPoint = ComputeIntersection(currentPoint, prevPoint, clipEdge);
                if (Inside(currentPoint, clipEdge))
                {
                    if (!Inside(prevPoint, clipEdge))
                    {
                        outputList.Add(intersectingPoint);
                    }

                    outputList.Add(currentPoint);
                } else if (Inside(prevPoint, clipEdge))
                {
                    outputList.Add(intersectingPoint);
                }
            }
            
        }

        return outputList.ToArray();
    }

    public static bool Inside(Vector2 p, Poly2DEdge line)
    {
        return (line.B.X - line.A.X) * (p.Y - line.A.Y) > (line.B.Y - line.A.Y) * (p.X - line.A.X);
    }

    public static bool FiniteLinesIntersect(Poly2DEdge a, Poly2DEdge b)
    {
        return Inside(a.A, b) != Inside(a.B, b) && Inside(b.A, a) != Inside(b.B, a);
    }
    
    public static Vector2 CollidesWith(this Polygon self, Polygon other)
    {
        Poly2DEdge[] edges = self.GetEdges();
        Poly2DEdge[] otherEdges = other.GetEdges();
        foreach (var edge in edges)
        {
            foreach (var otherEdge in otherEdges)
            {
                if (FiniteLinesIntersect(edge, otherEdge))
                    return ComputeIntersection(edge.A, edge.B, otherEdge);
            }
        }
        
        foreach (var point in self.Vertices)
        {
            if (other.Contains(point)) return point;
        }

        foreach (var point in other.Vertices)
        {
            if (self.Contains(point)) return point;
        }
        
        return new Vector2(float.NaN, float.NaN);
    }
    
    public static bool SH_CollidesWith(this Polygon self, Polygon other)
    {
        return SutherlandHodgman(self, other).Length > 1;
    }
}