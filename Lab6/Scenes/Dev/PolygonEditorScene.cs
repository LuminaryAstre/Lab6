using System;
using System.Collections.Generic;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using SharpHook;
using SharpHook.Data;

namespace Lab6.Scenes.Dev;

public class PolygonEditorScene : StarshipScene
{
    public override SamplerState PreferredSamplerState { get; set; } = SamplerState.PointClamp;
    public float PreviousZoom;
    public bool PreviousDebugCam;
    public List<Action> Scheduled = [];

    public List<Vector2> Points = [
        Vector2.One,
        Vector2.One + Vector2.UnitX * 16,
        Vector2.One * 16
    ];

    public Vector2 CursorPos = Vector2.One;

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Start()
    {
        PreviousZoom = Reactor._instance.Camera.Zoom;
        PreviousDebugCam = StarshipSettings.DebugCam;
        Reactor._instance.Camera.Zoom = 5;
        StarshipSettings.DebugCam = true;
        // Reactor._instance.IsMouseVisible = true;
        base.Start();
    }

    public override void Stop()
    {
        Reactor._instance.Camera.Zoom = PreviousZoom;
        StarshipSettings.DebugCam = PreviousDebugCam;
        // Reactor._instance.IsMouseVisible = false;
        base.Stop();
    }

    public override void OnClick(MouseHookEventArgs args)
    {
        if (args.Data.Button == MouseButton.Button1) Scheduled.Add(() => Points.Add(CursorPos));
        if (args.Data.Button == MouseButton.Button2) Scheduled.Add(() => Points.Clear());
        base.OnClick(args);
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var op in Scheduled.Copy())
        {
            op();
        }

        Scheduled.Clear();
        if (Keys.C.WasPressed()) Points.Add(CursorPos);
        if (Keys.V.WasPressed()) Points.Clear();
        if (Keys.B.WasPressed() || Keys.Enter.WasPressed())
        {
            string final = "[";
            foreach (var point in Points)
            {
                final += $"\n    new Vector2({point.X}f, {point.Y}f),";
            }

            final += "\n]";
            Console.WriteLine(final);
        }
        KeyboardState kb = Keyboard.GetState();
        // float stepSize = 1;
        // if (kb.IsKeyDown(Keys.Up)) CursorPos.Y -= stepSize;
        // if (kb.IsKeyDown(Keys.Down)) CursorPos.Y += stepSize;
        // if (kb.IsKeyDown(Keys.Left)) CursorPos.X -= stepSize;
        // if (kb.IsKeyDown(Keys.Right)) CursorPos.X += stepSize;
        CursorPos = Reactor._instance.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());
        if (kb.IsKeyDown(Keys.LeftShift)) CursorPos.Round();
        Reactor._instance.Text($"Points: {Points.Count}");
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        sprite.Draw(Reactor.Textures.Tripod, Vector2.Zero, Color.White);
        Polygon p = new Polygon(Points);
        sprite.DrawPolygon(Vector2.Zero, p, Color.Aqua, 1f);
        foreach (var point in Points)
        {
            sprite.DrawCircle(new CircleF(point, 2f), 6, Color.Green);
            sprite.DrawString(Reactor._instance.ReactorFont, $"({point.X}, {point.Y})", point, Color.White, 0, Vector2.Zero, Vector2.One * 0.25f, SpriteEffects.None, 0);
        }
        
        sprite.DrawCircle(new CircleF(CursorPos, 2f), 6, Color.Yellow);
        base.Draw(sprite, gameTime);
    }
}