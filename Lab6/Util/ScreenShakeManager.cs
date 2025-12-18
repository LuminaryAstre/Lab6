using System;
using Microsoft.Xna.Framework;

namespace Lab6.Util;

public class ScreenShakeManager()
{
    public float Intensity = 0f;
    private Random _random = new Random();

    public void Shake(float intensity)
    {
        Intensity = Math.Max(Intensity, intensity);
    }

    public void Tick(GameTime time)
    {
        if (Intensity > 0f)
        {
            var mat = Matrix.Identity;
            mat.Translation = (Vector3.UnitX + Vector3.UnitY) * new Vector3(_random.NextSingle(), _random.NextSingle(), _random.NextSingle()) * Intensity;
            Reactor._instance.CameraMatrixOffset *= mat;
            Intensity *= 0.98f;
            if (Intensity <= 0.1f) Intensity = 0;
        }
    }
}