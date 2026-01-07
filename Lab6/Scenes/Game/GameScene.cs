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
    public CoinObject Coin;
    public int Score = 0;
    public int HighScore;
    public Waveinator Wave;

    public override void Start()
    {
        Wave = new();
        Wave.PlayFile("./Content/project_starship_ingame.ogg", TimeSpan.Zero);
        
        Player = new PlayerObject("Player", Reactor.Textures.Ship, CommonColliderShapes.ShipCollider.ToCollisionMesh());
        Player.Center();
        Tracker.Track(Player);
        Coin = new CoinObject("Coin", Reactor.Textures.Coin, CommonColliderShapes.CoinCollider.ToCollisionMesh());
        Coin.Center();
        Tracker.Track(Coin);

        for (int i = 0; i < Reactor.Random.Next(4, 8); i++)
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

        HighScore = Persistinator.ReadInt("high_score");
        
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

        if (Player.Health == 0) Reactor._instance.SetActive(new MenuWrapper().SetActive(new GameOverScene(Score)));
        if (Score > HighScore)
        {
            Persistinator.WriteToFile("high_score", Score.ToString());
            HighScore = Score;
        }
        base.Update(gameTime);
    }

    public override void Stop()
    {
        Wave.Cease();
        base.Stop();
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        Tracker.Draw(sprite);
        Reactor._instance.Text($"{Player.Health}/{Player.MaxHealth} HP");
        Reactor._instance.Text($"{Score} Points");
        Reactor._instance.Text($"{HighScore} High score");
        base.Draw(sprite, gameTime);
    }
}