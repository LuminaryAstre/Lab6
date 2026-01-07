#if DEBUG
using System.Collections.Generic;
using Lab6.Objects;
using Lab6.Physics;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Lab6.Scenes.Dev;

public class ColliderTestScene : StarshipScene
{
    public Texture2D TestTexture;
    public override SamplerState PreferredSamplerState { get; set; } = SamplerState.PointWrap;
    public PhysicsObject TestObj;
    public List<HitInfo> Hits = [];
    public float Resolution = 1f;
    // more clear on block size, less overall parsable.
    public bool Grid = false;
    public int Samples = 0;
    public int ActiveSamples = 0;
    

    public override void LoadContent()
    {
        TestTexture = Content.Load<Texture2D>("ship");
        base.LoadContent();
    }

    public override void Start()
    {
        base.Start();

        TestObj = new PlayerObject("Test", TestTexture, CollisionComplexityUtil.GetCollision(CommonColliderShapes.ShipCollider));
        TestObj.SetPosition(new Vector2(Reactor._instance.Width, Reactor._instance.Height) / 2);
    }

    public void CalculateCollision(bool cheap = false)
    {
        float v = 1 / Resolution;
        Polygon point = new Polygon([Vector2.Zero, Vector2.UnitX * v, Vector2.One * v, Vector2.UnitY * v]);
        Hits.Clear();
        Samples = 0;
        ActiveSamples = 0;
        RectangleF rect = TestObj.Collision.GetTransformedShape().BoundingRectangle;
        float maxDist = new Vector2(rect.Size.Width, rect.Size.Height).Length()/7.5f;
        for (float x = 0; x < Reactor._instance.Width; x += v)
        {
            for (float y = 0; y < Reactor._instance.Height; y += v)
            {
                Samples++;
                var p = new Vector2(x, y);
                if (rect.DistanceTo(new Vector2(x,y)) > maxDist)
                {
                    Hits.Add(new HitInfo()
                    {
                        DidHit = false,
                        Pos = p,
                        Active = false
                    });
                    continue;
                };
                ActiveSamples++;
                bool doesHit = TestObj.CheckCollision(new CollisionMesh(point)
                {
                    Position = p
                }, cheap);
                Hits.Add(new HitInfo()
                {
                    DidHit = doesHit,
                    Pos = p,
                    Active = true
                });
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        bool up = Keys.Up.IsDown();
        bool down = Keys.Down.IsDown();
        bool left = Keys.Left.IsDown();
        bool right = Keys.Right.IsDown();
        Reactor._instance.Text("[F]: Toggle Grid");
        Reactor._instance.Text("[G]: Calculate Collision (Poly2D)");
        Reactor._instance.Text("[H]: Calculate Collision (RectF)");
        if (Keys.F.WasPressed())
            Grid = !Grid;
        if (Keys.G.WasPressed())
            CalculateCollision();
        if (Keys.H.WasPressed())
            CalculateCollision(true);
        
        Vector2 wishVelocity = -Vector2.UnitY * 3;
        wishVelocity.Rotate(TestObj.Rotation.ToRadians());
        
        // if (up) TestObj.DeltaMovement += wishVelocity;
        // if (down) TestObj.DeltaMovement -= wishVelocity/2;
        // if (left) TestObj.DeltaRotation -= 6;
        // if (right) TestObj.DeltaRotation += 6;
        
        TestObj.Tick(gameTime);


        Reactor._instance.Text($"Collision samples: {Samples}");
        Reactor._instance.Text($"Active samples: {ActiveSamples}");
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        TestObj.Draw(Reactor._instance.ReactorFont, sprite, false);
        float f = (1 / Resolution);
        foreach (var hit in Hits)
        {
            if (!hit.Active) continue;
            sprite.DrawPoint(hit.Pos + (Grid ? new Vector2(f / 4, f / 4) : Vector2.Zero), (hit.DidHit ? Color.Green : hit.Active ? Color.Red : Color.DimGray) * new Color(255,255,255,64), Grid ? f/2 : f);
        }
        base.Draw(sprite, gameTime);
    }
}
#endif