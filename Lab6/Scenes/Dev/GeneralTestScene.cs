#if DEBUG
using Lab6.Objects;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;

namespace Lab6.Scenes.Dev;

public class GeneralTestScene : StarshipScene
{
    public PhysicsObject ObjectOne;
    public PhysicsObject ObjectTwo;

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Start()
    {
        Polygon coll = new Polygon(
            [
                new Vector2(21.865242f, 14.279266f),
                new Vector2(27.302376f, 18.7658f),
                new Vector2(28.933517f, 1.0915756f),
                new Vector2(38.040726f, 9.384865f),
                new Vector2(40.215584f, 4.8983307f),
                new Vector2(42.798225f, 21.212997f),
                new Vector2(42.526367f, 39.023186f),
                new Vector2(34.642517f, 55.337852f),
                new Vector2(29.06945f, 51.667053f),
                new Vector2(22.273026f, 53.84234f),
                new Vector2(14.661034f, 51.938965f),
                new Vector2(8.952034f, 55.881676f),
                new Vector2(1.2041092f, 40.24678f),
                new Vector2(0.7963257f, 20.261314f),
                new Vector2(2.8352509f, 4.762375f),
                new Vector2(4.8741837f, 10.608467f),
                new Vector2(14.525101f, 0.9556198f),
                new Vector2(15.884384f, 17.81411f),
            ]
        );
        ObjectTwo = new PhysicsObject("B", Reactor.Textures.Ship, coll.ToMesh());
        ObjectTwo.SetPosition(Reactor._instance.Camera.Center);
        ObjectOne = new PhysicsObject("A", Reactor.Textures.Ship, coll.ToMesh());
        base.Start();
    }

    public override void Update(GameTime gameTime)
    {
        bool up = Keys.Up.IsDown();
        bool down = Keys.Down.IsDown();
        bool left = Keys.Left.IsDown();
        bool right = Keys.Right.IsDown();
        
                
        Vector2 wishVelocity = -Vector2.UnitY * 3;
        wishVelocity.Rotate(ObjectOne.Rotation.ToRadians());
        
        if (up) ObjectOne.DeltaMovement += wishVelocity;
        if (down) ObjectOne.DeltaMovement -= wishVelocity/2;
        if (left) ObjectOne.DeltaRotation -= 5;
        if (right) ObjectOne.DeltaRotation += 5;
        ObjectTwo.Tick(gameTime);
        ObjectOne.Tick(gameTime);
        
        
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        ObjectOne.Draw(Reactor._instance.ReactorFont, sprite);
        ObjectTwo.Draw(Reactor._instance.ReactorFont, sprite);
        Reactor._instance.Text($"Collision: {ObjectOne.CheckCollision(ObjectTwo)}");

        base.Draw(sprite, gameTime);
    }
}
#endif