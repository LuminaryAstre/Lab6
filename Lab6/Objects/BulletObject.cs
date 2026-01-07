using System;
using Lab6.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab6.Objects;

public class BulletObject : PhysicsObject
{
    public int TicksUntilDeath = 100;
    
    public BulletObject(string name, Texture2D sprite) : base(name, sprite)
    {
        DeltaDecay = false;
        EdgeInteractionType = EdgeInteractionType.Destroy;
        Collision = new CircleCollisionMesh(5);
    }

    public override void Tick(GameTime time)
    {
        if (TicksUntilDeath <= 0)
        {
            Destroy();
            return;
        }
        TicksUntilDeath -= 1;
        base.Tick(time);
    }

    public override void OnCollide(PhysicsObject other)
    {
        if (!(other is PlayerObject || other is BulletObject))
        {
            DeferDestroy();
        }
    }
}