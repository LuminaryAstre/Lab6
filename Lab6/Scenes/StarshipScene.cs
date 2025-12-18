using System;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpHook;

namespace Lab6.Scenes;

public class StarshipScene : IDisposable
{
    public bool IsDisposed = false;
    public ContentManager Content;
    public GraphicsDeviceManager Graphics;
    public ScreenShakeManager ScreenShake;
    public virtual SamplerState PreferredSamplerState { get; set; } = null;
    public bool Started;

    public StarshipScene()
    {
        Content = new ContentManager(Reactor._instance.Content.ServiceProvider);
        Content.RootDirectory = Reactor._instance.Content.RootDirectory;
    }
    
    ~StarshipScene() => Dispose(false);

    public virtual void Initialize()
    {
        LoadContent();
        ScreenShake = new ScreenShakeManager();
    }
    
    public virtual void OnClick(MouseHookEventArgs args) { }
    
    public virtual void LoadContent() { }

    public virtual void UnloadContent()
    {
        Content.Unload();
    }
    
    public virtual void Start() { }
    
    public virtual void Stop() { }

    public virtual void Update(GameTime gameTime)
    {
        ScreenShake.Tick(gameTime);
    }

    public virtual void Draw(SpriteBatch sprite, GameTime gameTime) { }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }
    }
}