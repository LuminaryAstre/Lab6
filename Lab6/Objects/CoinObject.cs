using Lab6.Physics;
using Lab6.Scenes.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab6.Objects;

public class CoinObject(string name, Texture2D sprite, ICollisionMesh coll = null) : PhysicsObject(name, sprite, coll)
{
    public override void OnCollide(PhysicsObject other)
    {
        if (other is PlayerObject plr)
        {
            if (Reactor.CurrentScene is GameScene game)
            {
                game.Score += 1;
                Center();
            }
        }
        base.OnCollide(other);
    }

    public override void Center(bool preserveDeltaMov = false)
    {
        SetPosition(
            new Vector2(Reactor._instance.Width * Reactor.Random.NextSingle(),
                Reactor._instance.Height * Reactor.Random.NextSingle()), preserveDeltaMov);
    }
}