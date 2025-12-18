using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Lab6.Util;

public class SfxEmitter(ContentManager content)
{
    public Sfx ShootSfx = new("Weapon fired.", "school_spaceship_shoot", content);
    public Sfx HitSfx = new("Ship takes damage.", "hit", content);
    
    public ContentManager Content = content;

    public void ShutUp()
    {
        ShootSfx.Stop();
        HitSfx.Stop();
    }
}

public class Sfx(string subtitle, string path, ContentManager content)
{
    public string Subtitle = subtitle;
    public string Path = path;
    private SoundEffect _asset;
    private SoundEffectInstance _instance;
    private ContentManager _content = content;

    public void Emit()
    {
        if (_instance == null)
        {
            _asset ??= _content.Load<SoundEffect>(Path);
            _instance = _asset.CreateInstance();
            _instance.Play();
        }
        else
        {
            _instance.Stop();
            _instance.Play();
        }
    }

    public void Stop()
    {
        if (_instance != null) _instance.Stop();
    }
}