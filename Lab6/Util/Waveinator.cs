using System;
using System.IO;
using System.Linq;
using NVorbis;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using SoundFlow.Structs;

namespace Lab6.Util;


/*
 * And so, at long last, came the difficult journey through
 * the hellscape of C# cross-platform sound libraries to an end.
 * As the traveller ventured into the abyss in search for one that
 * would do the bare minimum of working, he went more and more insane
 * as he kept running into issue after issue. He even ended up having
 * to monkeypatch the getter to RuntimeInformation.ProcessArchitecture
 * to try and get one of them, OwnAudioSharp, to load, but to no avail.
 * Eventually, he decided to go lower level and try out LibSoLoud, as
 * it had received praise of working well cross-platform, but alas, it
 * refused. At long last, another soul pointed him to a set of three
 * doors. Behind the first, a library called irrKlang. The traveller
 * considered it, but after going to download it, saw that it was neither
 * open source, nor on NuGet. The second door held libsoundio-sharp.
 * It's name sounded professional, surely the codebase and documentation
 * would be the same, no? But fate is cruel, and the documentation
 * sucked ass. Behind door three laid a library with no documentation
 * and low hopes, but alas, it too did not work. But lo, on hopes and
 * dreams it sends: VLC media player has bindings for C#. Truly, this
 * looked like what the traveller had been seeking, for it played sound!
 * Except it is incredibly jank. Repeat doesn't work (certainly not gapless),
 * and an all-around pain to use. Was this fate? Perhaps SoundFlow was
 * the answer. The traveller warily added the package to his project
 * to try it. And... It worked. The implementation may be overengineered,
 * but finally, seamless looping is at hand. No more, shall the
 * traveller suffer audio.
 *
 * Audio in C# is not fun.
 */
public class Waveinator : IDisposable
{
    public MiniAudioEngine Engine;
    public AudioPlaybackDevice PlaybackDevice;
    public SoundPlayer Player;
    public VorbisReader Data;

    ~Waveinator()
    {
        Dispose(false);
    }

    public void Cease()
    {
        Player.Stop();
        PlaybackDevice.Stop();
        Dispose();
    }

    public void PlayFile(string soundClip, TimeSpan? loopStart = null)
    {
        Data = DecodeVorbis(soundClip);
        loopStart ??= Data.TotalTime/2;
        float[] pcmIntro = GetPcm(Data);
        Engine = new MiniAudioEngine();
        var formatIntro = new AudioFormat
        {
            Channels = Data.Channels,
            SampleRate = Data.SampleRate,
            Format = SampleFormat.F32
        };

        PlaybackDevice = Engine.InitializePlaybackDevice(
            Engine.PlaybackDevices.FirstOrDefault(x => x.IsDefault), 
            formatIntro);
        
        Player = new SoundPlayer(Engine, PlaybackDevice.Format,
            new RawDataProvider(pcmIntro));
        
        PlaybackDevice.MasterMixer.AddComponent(Player);
        PlaybackDevice.Start();
        Player.IsLooping = true;
        Player.SetLoopPoints((TimeSpan)loopStart);
        Player.Play();
    }

    private static VorbisReader DecodeVorbis(string path)
    {
        var f = File.OpenRead(path);
        var decoded = new VorbisReader(f);
        return decoded;
    }
    private static float[] GetPcm(VorbisReader decoded)
    {
        Span<float> buf = new Span<float>(new float[decoded.TotalSamples * decoded.Channels]);
        decoded.ReadSamples(buf);
        return buf.ToArray();
    }
    

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Engine?.Dispose();
            PlaybackDevice?.Dispose();
            Player?.Dispose();
            Data?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}