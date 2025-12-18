using System;
using Lab6.Objects;
using Lab6.Physics;
using Lab6.Scenes.Menu;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace Lab6.Scenes.Game;

public class GameScene : StarshipScene
{
    public EntityTracker Tracker = new();
    public PlayerObject Player; 
    public int Score = 0;

    public override void Start()
    {
        Player = new PlayerObject("Player", Reactor.Textures.Ship, CommonColliderShapes.ShipCollider.ToCollisionMesh());
        Player.Center();
        Tracker.Track(Player);

        for (int i = 0; i < 5; i++)
        {
            var obstacle = new LivingObject("Meteor", Reactor.Textures.Stone, new CircleCollisionMesh(25))
            {
                ContactDamage = 1,
                DeltaDecay = false
            };
            Tracker.Track(obstacle);
            Random.Shared.NextUnitVector(out obstacle.DeltaMovement);
            obstacle.DeltaMovement *= Random.Shared.NextSingle();
            obstacle.DeltaMovement.X = MathF.Abs(obstacle.DeltaMovement.X) * 6f;
            obstacle.DeltaRotation = (Random.Shared.NextSingle() - .5f) * 180f;
            Random.Shared.NextUnitVector(out var pos);
            pos = pos * new Vector2(Reactor._instance.Width, Reactor._instance.Height);
            obstacle.Position = pos;
        }
        base.Start();
    }

    public override void Update(GameTime gameTime)
    {
        Tracker.Tick(gameTime);
        KeyboardStateExtended kb = KeyboardExtended.GetState();
        if (StarshipKeybindings.Shoot.WasKeyPressed(kb))
        {
            for (int i = -1; i <= 1; i++)
            {
                BulletObject bullet = new BulletObject("Bullet", Reactor.Textures.Bullet);
                var dir = -Vector2.UnitY.Rotated((Player.Rotation + 25f * i).ToRadians());
                bullet.DeltaMovement = dir * 8f + Player.DeltaMovement;
                bullet.Position = Player.Position + dir * 3f;
                bullet.Rotation = Player.Rotation;
                Tracker.Track(bullet);
            }
        }

        if (Player.Health == 0) Reactor._instance.SetActive(new MenuWrapper().SetActive(new MenuScene()));
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        Tracker.Draw(sprite);
        Reactor._instance.Text($"{Player.Health}/{Player.MaxHealth} HP");
        base.Draw(sprite, gameTime);
    }
}