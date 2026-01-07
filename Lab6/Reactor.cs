using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Lab6.Scenes;
using Lab6.Scenes.Menu;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;
using SharpHook;
using SharpHook.Data;
using MouseButton = SharpHook.Data.MouseButton;

// ReSharper disable InconsistentNaming

namespace Lab6;

public class Reactor : Game
{
    public static Reactor _instance;
    public static Random Random = new Random();
    
    private static StarshipScene _activeScene;
    private static StarshipScene _nextScene;
    public static StarshipScene CurrentScene => _activeScene;
    public SpriteFont ReactorFont;
    public int Width;
    public int Height;
    
    public OrthographicCamera Camera;

    public EventLoopGlobalHook ManIJustWantReliableMouseClickDetectionButImTooFastForMonogameToNotice;
    
    public GraphicsDeviceManager Graphics;
    public SpriteBatch Sprite;
    public bool Ticking = false;
    public Matrix CameraMatrixOffset;
    
    public int ExitingTicks = 0;
    public int TargetExitingTicks = 120;
    public List<string> QueuedText = [];
    public static Textile Textures;
    public static SfxEmitter SoundEmitter;
    
    public Reactor()
    {
        if (_instance is not null)
            throw new InvalidOperationException("YOU FOOL! This spaceship can only handle the power output of ONE reactor!");
        
        _instance = this;
        
        Graphics = new GraphicsDeviceManager(this);
        Graphics.HardwareModeSwitch = false;
        Graphics.ApplyChanges();
        RecalculateViewport();
        ManIJustWantReliableMouseClickDetectionButImTooFastForMonogameToNotice =
            new EventLoopGlobalHook(GlobalHookType.Mouse);
        Window.Title = "Starship";
        Window.AllowUserResizing = true;
        Content.RootDirectory = "Content";
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1/60f);
        
        Window.ClientSizeChanged += (sender, args) => RecalculateViewport();
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        ManIJustWantReliableMouseClickDetectionButImTooFastForMonogameToNotice.Stop();
        ManIJustWantReliableMouseClickDetectionButImTooFastForMonogameToNotice.Dispose();
        base.OnExiting(sender, args);
    }

    private void _click(Object? sender, MouseHookEventArgs args)
    {
        if (!IsActive) return;
        _activeScene?.OnClick(args);
    }


    protected void RecalculateViewport()
    {
        if (Camera != null)
        {
            Width = (int)Camera.BoundingRectangle.Width;
            Height = (int)Camera.BoundingRectangle.Height;
            return;
        }
        Width = Graphics.GraphicsDevice.Viewport.Width;
        Height = Graphics.GraphicsDevice.Viewport.Height;
    }

    protected override void LoadContent()
    {
        ReactorFont = Content.Load<SpriteFont>("Reactor");
        Textures = new Textile(Content);
        SoundEmitter = new SfxEmitter(Content);
        base.LoadContent();
    }

    protected override void Initialize()
    {
        base.Initialize();
        Persistinator.Setup();
        // DisplayMode dsp = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        Rectangle dsp = new Rectangle(0, 0, 1280, 800);
        Graphics.PreferredBackBufferHeight = dsp.Height;
        Graphics.PreferredBackBufferWidth = dsp.Width;
        Graphics.ApplyChanges();
        RecalculateViewport();

        ViewportAdapt = new BoxingViewportAdapter(Window, GraphicsDevice, dsp.Width, dsp.Height);
        Camera = new OrthographicCamera(ViewportAdapt);
        LockNKey.Initialize();
        ManIJustWantReliableMouseClickDetectionButImTooFastForMonogameToNotice.MouseClicked += _click;
        ManIJustWantReliableMouseClickDetectionButImTooFastForMonogameToNotice.RunAsync();
        Sprite = new SpriteBatch(Graphics.GraphicsDevice);
        CameraMatrixOffset = Matrix.Identity;
    }

    public BoxingViewportAdapter ViewportAdapt;

    public void SetActive(StarshipScene scene)
    {
        scene.Graphics = Graphics;
        _nextScene = scene;
    }

    public void ResetCam()
    {
        Camera.Zoom = 1f;
        Camera.Position = Vector2.Zero;
    }

    protected override void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        Ticking = true;
        CameraMatrixOffset = Matrix.Identity;
        QueuedText.Clear();
        // Tick helper classes
        LockNKey.Tick();
        MouseExtended.Update();
        KeyboardExtended.Update();
        KeyboardState kb = Keyboard.GetState();

        if (StarshipSettings.DebugCam)
        {
            
            var mv = Vector2.Zero;
            var state = Keyboard.GetState();
            if (MouseExtended.GetState().IsButtonDown(MonoGame.Extended.Input.MouseButton.Right))
            {
                mv += MouseExtended.GetState().DeltaPosition.ToVector2() / Camera.Zoom;
                Text(MouseExtended.GetState().DeltaPosition.ToVector2().ToVectorString());
            }
            Camera.Move(mv);
            float zoomPerTick = 0.01f;
            float scroll = MouseExtended.GetState().DeltaScrollWheelValue;
            Camera.ZoomIn(zoomPerTick * scroll * 0.01f);

            if (state.IsKeyDown(Keys.R))
                ResetCam();
        }
        else
            ResetCam();

        if (_activeScene is not ErrorBoundaryScene)
            try
            {
                if (kb.IsKeyDown(Keys.Escape) && _activeScene is not MenuWrapper)
                {
                    ExitingTicks++;
                    if (ExitingTicks >= TargetExitingTicks) SetActive(new MenuWrapper().SetActive(new MenuScene()));
                }
                else
                    ExitingTicks = 0;

                if (_nextScene is not null)
                    GotoNextScene();
                if (_activeScene is not null)
                {
                    if (!_activeScene.Started)
                    {
                        _activeScene.Start();
                        _activeScene.Started = true;
                    }

                    _activeScene.Update(gameTime);
                }
            }
            catch (Exception e)
            {
                SetActive(new ErrorBoundaryScene(e));
            }
        else
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) Exit();
        }

        base.Update(gameTime);
        Ticking = false;
    }

    public void Text(string msg)
    {
        QueuedText.Add(msg);
    }
    

    protected override void Draw(GameTime gameTime)
    {
        if (!IsActive) return;
        Graphics.GraphicsDevice.Clear(Color.Black);
        try
        {
            if (_activeScene is not null)
            {
                Sprite.Begin(samplerState: _activeScene.PreferredSamplerState, transformMatrix: Camera.GetViewMatrix() * CameraMatrixOffset);
                _activeScene.Draw(Sprite, gameTime);
                if (StarshipSettings.DebugCam)
                    Sprite.DrawRectangle(new RectangleF(Vector2.Zero, new SizeF(Camera.BoundingRectangle.Width * Camera.Zoom, Camera.BoundingRectangle.Height * Camera.Zoom)), Color.Red, 1/Camera.Zoom);
                Sprite.End();
            }
        }
        catch (Exception e)
        {
            SetActive(new ErrorBoundaryScene(e));
        }
        
        // Reactor's overlay
        Sprite.Begin(samplerState:SamplerState.PointClamp);
        if (StarshipSettings.DebugCam)
        {
            Text($"{Camera.Position} | {Camera.Zoom}");
        }

        if (ExitingTicks > 0)
        {
            float v = MathF.Min(1, ExitingTicks / 15f);
            Color color = new Color(v,v,v,v);
            Sprite.DrawString(ReactorFont, "Exiting... " + MathF.Round(ExitingTicks / (float)TargetExitingTicks*100) + "%", Vector2.One, color);
        }

        float offset = 0;
        foreach (var msg in QueuedText)
        {
            Sprite.DrawString(ReactorFont, msg, Vector2.One + Vector2.UnitY * (16 + offset), Color.White);
            offset += 16;
        }
        QueuedText.Clear();

        Sprite.End();
        base.Draw(gameTime);
    }

    private void GotoNextScene()
    {
        if (_activeScene is not null)
        {
            _activeScene.Stop();
            _activeScene.Dispose();
        }

        GC.Collect();
        SoundEmitter.ShutUp();
        _activeScene = _nextScene;
        _nextScene = null;

        try
        {
            if (_activeScene is not null)
                _activeScene.Initialize();
        }
        catch (Exception e)
        {
            SetActive(new ErrorBoundaryScene(e));
        }
    }
}
