using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab6.Scenes.Menu;

public class IntroScene : StarshipScene
{
    public int TicksPassed = 0;
    public override SamplerState PreferredSamplerState { get; set; } = SamplerState.PointClamp;

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Start()
    {
        Reactor._instance.SetActive(new MenuWrapper().SetActive(new MenuScene()));
        base.Start();
    }

    public override void Update(GameTime gameTime)
    {
        if (TicksPassed >= 500) Reactor._instance.SetActive(new MenuWrapper().SetActive(new MenuScene()));
        TicksPassed++;
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        Point res = new Point(Reactor._instance.Width, Reactor._instance.Height);
        sprite.Draw(Reactor.Textures.Logo, new Rectangle(new Point(100, (res.Y - 200)/4), new Point(200,200)), Color.White);
        sprite.DrawString(Reactor._instance.ReactorFont, "todo: make this good", new Vector2(325, (res.Y-200)/2f), Color.White, 0f, Vector2.Zero, Vector2.One * 4f, SpriteEffects.None, 0);
        base.Draw(sprite, gameTime);
    }
}