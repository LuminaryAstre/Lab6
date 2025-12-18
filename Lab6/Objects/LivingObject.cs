using System;
using Lab6.Physics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Lab6.Objects;

public class LivingObject(string name, Texture2D sprite, ICollisionMesh coll = null) : PhysicsObject(name, sprite, coll)
{
    public int Health = 1;
    public int MaxHealth = 1;
    public int ContactDamage = 0;
    public int TimeSinceLastHit = Int32.MaxValue;
    public bool Invulnerable = false;

    public virtual void OnDamage(int amount, LivingObject? other)
    {
        if (!Invulnerable) Health -= amount;
    }
    
    public override void OnCollide(PhysicsObject other)
    {
        if (other is LivingObject otherL)
        {
            OnDamage(otherL.ContactDamage, otherL);
        }
        DeltaMovement = (Position - other.Position).NormalizedCopy() * 4;
        base.OnCollide(other);
    }
}