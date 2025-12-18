using System;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Lab6.Objects;

public class StarParticleObject : PhysicsObject
{
    public Color Col;
    public float Alpha;
    public float TargetAlpha;
    public StarParticleObject(string name, Texture2D texture) : base(name, texture)
    {
        DeltaDecay = false;
        Random random = Random.Shared;
        Alpha = random.NextSingle(0,255);
        TargetAlpha = 255;
        random.NextUnitVector(out Position);
        Position *= new Vector2(Reactor._instance.Width, Reactor._instance.Height);
        random.NextUnitVector(out DeltaMovement);
        DeltaMovement *= .3f;
        // Col = new(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), 255);
        Col = new Color(255, 255, 255);
        EdgeInteractionType = EdgeInteractionType.Wrap;
    }

    public override void Tick(GameTime time)
    {
        Alpha = Alpha.Lerp(TargetAlpha, time.GetElapsedSeconds() * 0.5f);
        Alpha = Math.Clamp(Alpha, 0, 255);
        if (Math.Abs(Alpha - TargetAlpha) < 5)
        {
            switch (TargetAlpha)
            {
                case 255:
                    TargetAlpha = 0;
                    break;
                case 0:
                default:
                    Random.Shared.NextUnitVector(out Position);
                    Position *= new Vector2(Reactor._instance.Width, Reactor._instance.Height);
                    TargetAlpha = 255;
                    break;
            }
        }
        base.Tick(time);
    }

    public override void Draw(SpriteFont font, SpriteBatch batch, bool preserveLoopIllusion = false)
    {
        if (Destroyed) return;
        Col.ToHSL(out float h, out float s, out float l);
        Col = Col.FromHSL(h, s, Alpha);
        bool debug = StarshipSettings.DebugObjects;
        float drawRot = (Rotation / 360) * MathF.PI * 2;
        batch.Draw(Sprite, Position, null, debug ? Color.Green : Col, drawRot, Sprite.Bounds.Center.ToVector2(), 1.0f, SpriteEffects.None, 0f);
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
                Color col = Col;
                if (debug)
                    col = offset.Length() % 1 == 0 ? Color.BlueViolet : Color.Red;
                batch.Draw(Sprite, Position + offset, null, col, drawRot, Sprite.Bounds.Center.ToVector2(), 1.0f, SpriteEffects.None, 0f);
                if (debug) batch.DrawString(font, Name + " (loop illusion)", Position + offset + (Vector2.UnitX * Sprite.Width / 2), Color.White);
            }
        }

        if (debug)
            batch.DrawString(font, Name, Position + (Vector2.UnitX * Sprite.Width / 2), Color.White);

    }
}