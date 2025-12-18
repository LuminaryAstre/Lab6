using System;
using System.Collections.Generic;
using Lab6.Objects;
using Lab6.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpHook;

namespace Lab6.Scenes.Menu;

public class MenuWrapper : StarshipScene
{
    private StarshipScene _activeScene;
    private StarshipScene _nextScene;
    private static MenuWrapper _instance;
    public Waveinator Wave;
    public override SamplerState PreferredSamplerState => _activeScene?.PreferredSamplerState ?? SamplerState.PointClamp;

    public List<StarParticleObject> Particles = [];
    public static MenuWrapper Instance => _instance ?? throw new Exception("MenuWrapper.Instance is not valid at this point.");

    public MenuWrapper()
    {
        _instance = this;
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public MenuWrapper SetActive(StarshipScene scene)
    {
        scene.Graphics = Graphics;
        _nextScene = scene;
        return this;
    }
    
    public override void OnClick(MouseHookEventArgs args)
    {
        _activeScene?.OnClick(args);
        base.OnClick(args);
    }

    public override void Start()
    {
        Wave = new Waveinator();
        Wave.PlayFile("./Content/project_starship_menu.ogg");
        base.Start();
        Reactor._instance.ResetCam();
        for (int i = 0; i < 64; i++)
        {
            Particles.Add(new StarParticleObject("Star particle", Reactor.Textures.Particle));
        }
    }

    public override void Stop()
    {
        Wave.Cease();
        _activeScene?.Stop();
        _activeScene?.Dispose();
        _instance = null;
        base.Stop();
    }

    public override void UnloadContent()
    {
        _activeScene?.UnloadContent();
        base.UnloadContent();
    }

    private void GotoNextScene()
    {
        if (_activeScene is not null)
        {
            _activeScene.Stop();
            _activeScene.Dispose();
        }

        GC.Collect();
        _activeScene = _nextScene;
        _nextScene = null;

        if (_activeScene is not null)
            _activeScene.Initialize();
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach (var particle in Particles)
        {
            particle.Tick(gameTime);
        }
        if (_nextScene != null)
            GotoNextScene();
        
        if (_activeScene != null)
        {
            if (!_activeScene.Started)
            {
                _activeScene.Start();
                _activeScene.Started = true;
            }

            _activeScene.Update(gameTime);
        }
        base.Update(gameTime);
    }

    public override void Draw(SpriteBatch sprite, GameTime gameTime)
    {
        sprite.End();
        sprite.Begin(samplerState: SamplerState.AnisotropicClamp, transformMatrix: Reactor._instance.Camera.GetViewMatrix() * Reactor._instance.CameraMatrixOffset);
        foreach (var particle in Particles)
        {
            particle.Draw(Reactor._instance.ReactorFont, sprite, true);
        }
        sprite.End();
        sprite.Begin(samplerState: PreferredSamplerState, transformMatrix: Reactor._instance.Camera.GetViewMatrix() * Reactor._instance.CameraMatrixOffset);
        _activeScene?.Draw(sprite, gameTime);
        base.Draw(sprite, gameTime);
    }
}