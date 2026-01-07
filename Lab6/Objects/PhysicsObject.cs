using System;
using Lab6.Physics;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Lab6.Objects;

public enum EdgeInteractionType
{
    Wrap = 1,
    Bounce = 2,
    Stop = 3,
    Destroy = 4
}

public class PhysicsObject(string name, Texture2D sprite, ICollisionMesh coll = null)
{
    public string Name = name;
    public Texture2D Sprite = sprite;
    public Vector2 Position = Vector2.Zero;
    public Vector2 DeltaMovement = Vector2.Zero;
    public float DeltaRotation = 0;
    public float Rotation = 0;
    public bool DeltaDecay = true;
    public bool WillDestroy = false;
    public float DampingFactor = 0.02f;
    public EdgeInteractionType EdgeInteractionType = EdgeInteractionType.Wrap;
    public ICollisionMesh Collision = coll ?? new CollisionMesh(sprite.Bounds.ToPolygon());
    public bool Destroyed { get; private set; } = false;

    public virtual PhysicsObject SetPosition(Vector2 pos, bool preserveDeltaMov = false)
    {
        Position = pos;
        if (!preserveDeltaMov) DeltaMovement = Vector2.Zero;
        return this;
    }

    public virtual void Center(bool preserveDeltaMov = false)
    {
        SetPosition(new Vector2(Reactor._instance.Width / 2f, Reactor._instance.Height / 2f), preserveDeltaMov);
    }

    public virtual PhysicsObject SetRotation(float rot, bool preserveDeltaRot = false)
    {
        Rotation = rot;
        if (!preserveDeltaRot) DeltaRotation = 0;
        return this;
    }

    public virtual void Destroy()
    {
        Destroyed = true;
    }

    public virtual void DeferDestroy()
    {
        WillDestroy = true;
    }


    protected virtual void TickMovement(Reactor game, GameTime time)
    {
        if (DeltaDecay) DeltaMovement *= 1 - DampingFactor;
        if (DeltaMovement.LengthSquared() <= 0.01f) DeltaMovement = Vector2.Zero;
        Vector2 newPos = (Position + DeltaMovement);
        Collision.Position = newPos;
        if (EdgeInteractionType == EdgeInteractionType.Wrap)
            Position = newPos.Wrap(game.Width, game.Height);
        else
        {
            if (newPos.X > game.Width || newPos.X < 0)
            {
                switch (EdgeInteractionType)
                {
                    case EdgeInteractionType.Stop:
                        DeltaMovement.X = 0;
                        // Hey I actually learnt something new!
                        // C# has a "goto case" statement!
                        goto case EdgeInteractionType.Bounce;
                    case EdgeInteractionType.Bounce:
                        DeltaMovement.X *= -1;
                        newPos.X = MathF.Min(MathF.Max(0, newPos.X), game.Width);
                        break;
                    case EdgeInteractionType.Destroy:
                        Destroy();
                        return;
                }
            }
            if (newPos.Y > game.Height || newPos.Y < 0)
            {
                switch (EdgeInteractionType)
                {
                    case EdgeInteractionType.Stop:
                        DeltaMovement.Y = 0;
                        goto case EdgeInteractionType.Bounce;
                    case EdgeInteractionType.Bounce:
                        DeltaMovement.Y *= -1;
                        newPos.Y = MathF.Min(MathF.Max(0, newPos.Y), game.Height);
                        break;
                    case EdgeInteractionType.Destroy:
                        Destroy();
                        return;
                }
            }

            Position = newPos;
        }
    }

    protected virtual void TickRotation(Reactor game, GameTime time)
    {
        if (DeltaDecay) DeltaRotation *= 1 - DampingFactor;
        if (MathF.Abs(DeltaRotation) < 0.1f) DeltaRotation = 0;
        Rotation += DeltaRotation;
        Rotation = Rotation.Modulo(360);
        Collision.Rotation = Rotation;
    }

    public virtual bool CheckCollision(PhysicsObject other)
    {
        return CheckCollision(other.Collision);
    }

    public virtual void OnCollide(PhysicsObject other)
    {
        
    }

    public virtual bool CheckCollision(ICollisionMesh other, bool cheap = false)
    {
        return cheap ? Collision.DetectCollideCheap(other) : !Collision.DetectCollidePos(other).IsNaN();
    }

    public virtual void Tick(GameTime time)
    {
        if (WillDestroy) Destroy();
        if (Destroyed) return;
        Reactor game = Reactor._instance;
        TickMovement(game, time);
        TickRotation(game, time);
    }

    public virtual Polygon GetTrueCollision()
    {
        return Collision.GetShape();
    }
    
    public virtual void Draw(SpriteFont font, SpriteBatch batch, bool preserveLoopIllusion = false)
    {
        if (Destroyed) return;
        bool debug = StarshipSettings.DebugObjects;
        float drawRot = (Rotation / 360) * MathF.PI * 2;
        batch.Draw(Sprite, Position, null, debug ? Color.Green : Color.White, drawRot, Sprite.Bounds.Center.ToVector2() , 1.0f, SpriteEffects.None, 0f);
        var curDel = DeltaMovement;
        var curDelR = DeltaRotation;
        var curPos = new Vector2(Position.X, Position.Y);
        var curRot = Rotation;
        if (debug)
        {
            if (DeltaDecay)
            {
                while (curDel.LengthSquared() > 0.01f)
                {
                    curDel *= 1 - DampingFactor;
                    curPos += curDel;
                }

                while (MathF.Abs(curDelR) > 0.1f)
                {
                    curDelR *= 1 - DampingFactor;
                    curRot += curDelR;
                    curRot = curRot.Modulo(360);
                }

                var pol = Collision.GetShape()
                    .RotatedAround(Collision.GetShape().BoundingRectangle.Center, curRot.ToRadians());
                batch.DrawPolygon(curPos - pol.BoundingRectangle.Center, pol, Color.BlueViolet);
            }
            else
            {
                batch.DrawLine(Position, Position + DeltaMovement, Color.Orange);
            }
        }
        Reactor game = Reactor._instance;
        if (preserveLoopIllusion)
        {
            Vector2[] offsets = [
                new(game.Width,0),
                new(-game.Width,0),
                new(0,game.Height),
                new(0,-game.Height),
                new(game.Width,game.Height),
                new(-game.Width,-game.Height),
                new(-game.Width,game.Height),
                new(game.Width,-game.Height)
            ];
            foreach (Vector2 offset in offsets)
            {
                Color col = Color.White;
                if (debug)
                    col = offset.Length() % 1 == 0 ? Color.BlueViolet : Color.Red;
                batch.Draw(Sprite, Position + offset, null, col, drawRot, Sprite.Bounds.Center.ToVector2(), 1.0f, SpriteEffects.None, 0f);
                if (debug)
                {
                    batch.DrawString(font, Name + " (loop illusion)", Position + offset + (Vector2.UnitX * Sprite.Width / 2), Color.White);
                    if (DeltaDecay)
                    {
                        var pol = Collision.GetShape()
                            .RotatedAround(Collision.GetShape().BoundingRectangle.Center, curRot.ToRadians());
                        batch.DrawPolygon(curPos - pol.BoundingRectangle.Center + offset, pol, Color.BlueViolet);
                    }
                    else
                    {
                        batch.DrawLine(Position, Position + DeltaMovement * 30, Color.Orange);
                    }
                }
            }
        }

        if (debug)
        {
            Collision.Draw(batch);
            batch.DrawString(font, Name, Position + (Vector2.UnitX * Sprite.Width / 2), Color.White);
        }

    }
}