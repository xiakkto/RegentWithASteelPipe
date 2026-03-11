using Godot;

namespace RegentWithASteelPipe;

internal static class PipeSfxService
{
    private const string ResourcePath = "res://RegentWithASteelPipe/audio/steel_pipe.ogg";

    private static readonly StringName SfxBusName = new("sfx");

    private static readonly StringName MasterBusName = new("Master");

    private static AudioStreamOggVorbis? _cachedStream;

    internal static bool TryPlay(float volume)
    {
        AudioStreamOggVorbis? stream = _cachedStream ??= AudioStreamOggVorbis.LoadFromFile(ResourcePath);
        if (stream == null)
        {
            Console.WriteLine($"[RegentWithASteelPipe] Failed to load audio stream: {ResourcePath}");
            return false;
        }

        if (Engine.GetMainLoop() is not SceneTree tree)
        {
            return false;
        }

        Node root = tree.CurrentScene ?? tree.Root;
        AudioStreamPlayer player = new()
        {
            Stream = stream,
            Bus = AudioServer.GetBusIndex(SfxBusName) >= 0 ? SfxBusName : MasterBusName,
            VolumeDb = ToVolumeDb(volume)
        };

        player.Finished += player.QueueFree;
        root.AddChild(player);
        player.Play();
        return true;
    }

    private static float ToVolumeDb(float volume)
    {
        if (volume <= 0f)
        {
            return -80f;
        }

        return Mathf.LinearToDb(volume);
    }
}
