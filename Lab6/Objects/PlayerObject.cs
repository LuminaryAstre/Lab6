using System;
using Lab6.Physics;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace Lab6.Objects;

public class PlayerObject : LivingObject
{
    public bool PhysicsMovement = StarshipSettings.PhysicsMovement;

    public PlayerObject(string name, Texture2D sprite, ICollisionMesh coll = null) : base(name, sprite, coll)
    {
        DeltaDecay = StarshipSettings.DeltaDecay;
        MaxHealth = 5;
        Health = MaxHealth;
    }

    public override void OnDamage(int amount, LivingObject other)
    {
        TimeSinceLastHit = 0;
        base.OnDamage(amount, other);
        if (!Invulnerable)
        {
            Reactor.CurrentScene.ScreenShake.Shake(15f);
            Reactor.SoundEmitter.HitSfx.Emit();
            if (other != null)
            {
                // DeltaMovement = (Position - other.Position).NormalizedCopy() * 4;
                DeltaRotation = 45;
            }
        }
        
    }

    public override void OnCollide(PhysicsObject other)
    {
        if (other is BulletObject) return;
        if (TimeSinceLastHit < 120) return;
        base.OnCollide(other);
    }

    public override void Tick(GameTime time)
    {
        TimeSinceLastHit = Math.Max(TimeSinceLastHit + 1, TimeSinceLastHit);
        var kb = KeyboardExtended.GetState();
        
        Vector2 mv = Vector2.Zero;
        Vector2 wishVelocity = -Vector2.UnitY * .3f;
        wishVelocity.Rotate(Rotation.ToRadians());
        
        if (StarshipKeybindings.MoveForward.IsKeyDown(kb)) mv += wishVelocity;
        if (StarshipKeybindings.MoveBackward.IsKeyDown(kb)) mv -= wishVelocity;
        DeltaMovement += mv;
        if (StarshipKeybindings.RotateCounterClockwise.IsKeyDown(kb)) 
        {
            DeltaRotation -= PhysicsMovement ? MathF.Max(0, DeltaMovement.Length() - .3f) / 60 : 5;
        }
        if (StarshipKeybindings.RotateClockwise.IsKeyDown(kb)) 
        {
            DeltaRotation += PhysicsMovement ? MathF.Max(0, DeltaMovement.Length() - .3f) / 60 : 5;
        }
        
        if (PhysicsMovement)
            base.Tick(time);
        else
        {
            Position = Position + DeltaMovement * 30;
            DeltaMovement = Vector2.Zero;
            Rotation = Rotation + DeltaRotation;
            DeltaRotation = 0;
            base.Tick(time);
        }
    }
}