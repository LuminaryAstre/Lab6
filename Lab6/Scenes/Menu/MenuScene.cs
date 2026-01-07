using System;
using System.Collections.Generic;
#if DEBUG
using Lab6.Scenes.Dev;
#endif
using Lab6.Scenes.Game;
using Lab6.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpHook;
using MouseButton = SharpHook.Data.MouseButton;

namespace Lab6.Scenes.Menu;

public class MenuScene : StarshipScene
{
    private bool _left;
    public float ButtonSize = 3f;
    public virtual string Title { get; set; } = "Project: Starship";
    public virtual List<StarshipButton> Buttons { get; set; } =
    [
        new()
        {
            Name = "Play",
            Outline = Color.Gold,
            Callback = (btn) => Reactor._instance.SetActive(new GameScene())
        },
        new()
        {
            Name = "Controls",
            Callback = _ => MenuWrapper.Instance.SetActive(new ControlsList())
        },
        #if DEBUG
            new()
            {
                Name = "Test",
                Outline = Color.White,
                Callback = _ => Reactor._instance.SetActive(new GeneralTestScene())
            },

            new()
            {
                Name = "Collider Test",
                Outline = Color.White,
                Callback = _ => Reactor._instance.SetActive(new ColliderTestScene())
            },

            new()
            {
                Name = "Polygon editor",
                Outline = Color.White,
                Callback = _ => Reactor._instance.SetActive(new PolygonEditorScene())
            },

            new()
            {
                Name = "Error Test",
                Callback = _ => throw new EntryPointNotFoundException("Example error")
            },
        #endif
        new()
        {
            Name = "Options",
            Callback = (btn) => MenuWrapper.Instance.SetActive(new OptionsScene())
        },

        new()
        {
            Name = "Quit",
            Callback = (btn) => Reactor._instance.Exit()
        }
    ];

    public override void Initialize()
    {
        Reactor._instance.IsMouseVisible = true;
        base.Initialize();
    }

    public override void Stop()
    {
        Reactor._instance.IsMouseVisible = false;
        base.Stop();
    }

    public override void OnClick(MouseHookEventArgs args)
    {
        if (args.Data.Button == MouseButton.Button1)
            _left = true;
        base.OnClick(args);
    }

    public override void Update(GameTime gameTime)
    {
        int yOffset = 0;
        foreach (var btn in Buttons)
        {
            Vector2 pos = GetButtonPosFromOffset(yOffset);

            if (IsHovering(pos, btn))
            {
                Mouse.SetCursor(MouseCursor.Hand);
                if (_left) btn.Callback(btn);
                break;
            }

            Mouse.SetCursor(MouseCursor.Arrow);
            yOffset += 64;
        }

        _left = false;
        base.Update(gameTime);
    }

    public Vector2 GetButtonPosFromOffset(int yOffset)
    {
        return Vector2.UnitX * 8 + Vector2.UnitY * (yOffset + 128);
    }

    public Vector2 GetButtonDims(StarshipButton button)
    {
        Vector2 minSize = Reactor._instance.ReactorFont.MeasureString(button.Name) * 1.66f + (Vector2.One * 8 + Vector2.UnitX * 4);
        return new Vector2(Math.Max(minSize.X, 64 * ButtonSize), Math.Max(minSize.Y, 8 * ButtonSize));
    }

    public void DrawButton(SpriteBatch sprite, StarshipButton button, int yOffset)
    {
        Vector2 pos = GetButtonPosFromOffset(yOffset);
        sprite.Draw9Slice(Reactor.Textures.Button, pos, GetButtonDims(button), IsHovering(pos, button) ? Color.Aquamarine : button.Outline, 2f, 2);
        sprite.DrawString(Reactor._instance.ReactorFont, button.Name, pos + Vector2.One * 8 + Vector2.UnitX * 4, Color.White, 0f, Vector2.Zero, 1.66f, SpriteEffects.None, 0);
    }

    public bool IsHovering(Vector2 buttonPos, StarshipButton button)
    {
        Rectangle buttonRect = new Rectangle(buttonPos.ToPoint(), (GetButtonDims(button) + (Vector2.One * 8 + Vector2.UnitX * 4)).ToPoint());
        Point pos = Reactor._instance.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2()).ToPoint();
        return buttonRect.Contains(pos) && button.Active;
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        sprite.DrawString(Reactor._instance.ReactorFont, Title, Vector2.UnitY * 64 + Vector2.UnitX * 8, Color.Aquamarine, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0);
        int yOffset = 0;
        foreach (var btn in Buttons)
        {
            DrawButton(sprite, btn, yOffset);
            yOffset += 64;
        }
        base.Draw(sprite, gameTime);
    }
}
