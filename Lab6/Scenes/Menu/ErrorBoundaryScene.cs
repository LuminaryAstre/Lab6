using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Lab6.Scenes.Menu;

public class ErrorBoundaryScene : StarshipScene
{
    public Exception Exc;
    public ErrorBoundaryScene(Exception exc)
    {
        Exc = exc;
    }

    public override void Initialize()
    {
        Rectangle dsp = new Rectangle(0, 0, 1000, 600);
        Reactor._instance.Graphics.PreferredBackBufferHeight = dsp.Height;
        Reactor._instance.Graphics.PreferredBackBufferWidth = dsp.Width;
        Reactor._instance.Window.AllowUserResizing = false;
        Reactor._instance.Graphics.IsFullScreen = false;
        Reactor._instance.Graphics.ApplyChanges();
        Reactor._instance.IsMouseVisible = true;
        base.Initialize();
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        sprite.End();
        sprite.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
        var bounds = Reactor._instance.Window.ClientBounds.ToRectangleF();
        
        bounds.Position = Vector2.Zero;
        var size = new Point((int)bounds.Width / 4, (int)bounds.Width / 4);
        sprite.DrawRectangle(bounds, Color.Crimson, 5);
        sprite.Draw(Reactor.Textures.Logo, new Rectangle(new Point((int)bounds.Center.X, (int)bounds.Center.Y) - new Point(size.X/2, size.Y/2), size), new Color(64,64,64,64));

        string excMsg = "Starship has run into an uncaught error. Press Enter to exit.\nThe details are displayed below.";
        
        excMsg += $"\n{Exc.GetType().FullName}: {Exc.Message}\n{Exc.StackTrace}";
        excMsg = excMsg.Replace(" in ", "\n      in ");
        
        sprite.DrawString(Reactor._instance.ReactorFont, excMsg, Vector2.UnitY * 40 + Vector2.UnitX * 10, Color.Crimson);
        base.Draw(sprite, gameTime);
    }
}