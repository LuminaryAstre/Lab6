using System.Collections.Generic;
using System.Linq;
using Lab6.Objects;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab6.Physics;

public class EntityTracker(IEnumerable<PhysicsObject> objects)
{
    public List<PhysicsObject> Objects = objects.ToList();
    public bool PreserveLoopIllusion = true;

    public EntityTracker() : this([])
    {
        
    }

    public void Track(PhysicsObject obj)
    {
        Objects.Add(obj);
    }

    public void Untrack(PhysicsObject obj)
    {
        Objects.Remove(obj);
    }

    public void Tick(GameTime gameTime)
    {
        Objects.Copy().ForEach(po =>
        {
            if (po.Destroyed) Objects.Remove(po);
            else po.Tick(gameTime);
        });
        Objects.ForEach(po =>
        {
            Objects.ForEach(o =>
            {
                if (po == o) return;
                if (po.CheckCollision(o))
                {
                    po.OnCollide(o);
                    o.OnCollide(po);
                }
            });
        });
    }

    public void Draw(SpriteBatch sprite)
    {
        Objects.ForEach(po => po.Draw(Reactor._instance.ReactorFont, sprite, PreserveLoopIllusion));
    }
}