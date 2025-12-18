using System;
using System.Collections.Generic;
using System.Linq;
using Lab6.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Lab6.Util;

public static class StarshipExtensions
{
    public static string ToVectorString(this Vector2 vec)
    {
        return $"Vector2({MathF.Round(vec.X, 2)}, {MathF.Round(vec.Y, 2)})";
    }

    public static bool IsDown(this Keys key)
    {
        return Keyboard.GetState().IsKeyDown(key);
    }

    public static bool WasPressed(this Keys key)
    {
        return Keyboard.GetState().WasPressed(key);
    }

    public static Vector2 ClampLength(this Vector2 vec, float min, float max)
    {
        float len = vec.Length();
        if (min <= len && len <= max) return vec;
        float x = vec.X;
        float y = vec.Y;
        if (len > max)
        {
            x = (x / len) * max;
            y = (y / len) * max;
        }
        else
        {
            x = (x / len) * min;
            y = (y / len) * min;
        }

        return new Vector2(x, y);
    }

    public static Vector2 Lerp(this Vector2 a, Vector2 b, float t)
    {
        return (a * (1 - t)) + (b * t);
    }

    public static float Lerp(this float a, float b, float t)
    {
        return (a * (1 - t)) + (b * t);
    }

    public static int Lerp(this int a, int b, float t)
    {
        return (int)((a * (1 - t)) + (b * t));
    }

    public static byte Lerp(this byte a, byte b, float t)
    {
        return (byte)((a * (1 - t)) + (b * t));
    }

    public static RectangleF Moved(this RectangleF orig, float X, float Y)
    {
        return new RectangleF(orig.X + X, orig.Y + Y, orig.Width, orig.Height);
    }

    public static RectangleF Moved(this RectangleF orig, Vector2 pos)
    {
        return orig.Moved(pos.X, pos.Y);
    }

    public static Polygon ToPolygon(this Rectangle rect)
    {
        return new Polygon([
            Vector2.Zero,
            Vector2.UnitX * rect.Size.ToVector2(),
            rect.Size.ToVector2(),
            Vector2.UnitY * rect.Size.ToVector2()
        ]);
    }

    public static Polygon Rotated(this Polygon orig, float rot)
    {
        var poly = new Polygon(orig.Vertices);
        poly.Rotate(rot);
        return poly;
    }

    public static Polygon RotatedAround(this Polygon poly, Vector2 orig, float rot)
    {
        var p = poly.Moved(-orig);
        p.Rotate(rot);
        p.Offset(orig);
        return p;
    }

    public static Polygon Moved(this Polygon orig, Vector2 offset)
    {
        return new Polygon(orig.Vertices.Select(vector2 => vector2 + offset));
    }

    public static float ToRadians(this float deg)
    {
        return deg / 360 * MathF.PI * 2;
    }

    public static float ToDegrees(this float rad)
    {
        return rad * 360 / MathF.PI / 2;
    }

    // "Please note that C# and C++'s % operator is actually NOT a modulo, it's remainder."
    // thank you C#, very cool.
    // https://stackoverflow.com/a/6400477
    public static float Modulo(this float a,float b)
    {
        return a - b * MathF.Floor(a / b);
    }
    
    public static Vector2 Wrap(this Vector2 vec, float x, float y)
    {
        return new Vector2(Modulo(vec.X, x), Modulo(vec.Y, y));
    }

    public static Vector2 Rotated(this Vector2 vec, float val)
    {
        Vector2 v = new Vector2(vec.X, vec.Y);
        v.Rotate(val);
        return v;
    }

    public static Poly2DEdge[] GetEdges(this Polygon self)
    {
        List<Poly2DEdge> ret = [];
        for (int i = 0; i < self.Vertices.Length; i++)
        {
            var current = self.Vertices[i];
            var next = self.Vertices[(i + 1) % self.Vertices.Length];
            ret.Add(new Poly2DEdge(current, next));
        }

        return ret.ToArray();
    }

    public static List<T> Copy<T>(this List<T> list)
    {
        return list.ToList();
    }

    public static ICollisionMesh ToMesh(this Polygon self)
    {
        return CollisionComplexityUtil.GetCollision(self.Vertices);
    }

    public static ICollisionMesh ToCollisionMesh(this Vector2[] self)
    {
        return new Polygon(self).ToMesh();
    }
}