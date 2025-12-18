using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lab6.Util;

public class Textile(ContentManager content)
{
    private readonly Dictionary<string, Texture2D> _loaded = new();
    public Texture2D NinesliceTest => Load("9stest");

    public Texture2D Bullet => Load("bullet");
    public Texture2D Button => Load("button");
    public Texture2D Coin => Load("coin");
    public Texture2D Logo => Load("logo");
    public Texture2D Mine => Load("mine");
    public Texture2D Particle => Load("particle");
    public Texture2D Ship => Load("ship");
    public Texture2D Stone => Load("stone");
    public Texture2D Tripod => Load("tripod");

    private Texture2D Load(string name)
    {
        if (_loaded.TryGetValue(name, out var tex))
            return tex;
        Console.WriteLine($"[Textile/{content.RootDirectory}] Loading lazy-loaded Texture2D {name}");
        var text = content.Load<Texture2D>(name);
        _loaded.Add(name, text);
        return text;
    }
}
