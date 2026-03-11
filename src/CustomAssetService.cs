using Godot;

namespace RegentWithASteelPipe;

internal static class CustomAssetService
{
    private const string CardPortraitPath = "res://RegentWithASteelPipe/images/steel_pipe_card.png";

    private const string BladeTexturePath = "res://RegentWithASteelPipe/images/steel_pipe_blade_runtime.png";

    private static Texture2D? _cardPortrait;

    private static Texture2D? _bladeTexture;

    internal static Texture2D? CardPortrait => _cardPortrait ??= LoadTexture(CardPortraitPath);

    internal static Texture2D? BladeTexture => _bladeTexture ??= LoadTexture(BladeTexturePath);

    private static Texture2D? LoadTexture(string path)
    {
        try
        {
            Image image = Image.LoadFromFile(path);
            if (image.GetWidth() <= 0 || image.GetHeight() <= 0)
            {
                Console.WriteLine($"[RegentWithASteelPipe] Failed to load texture: {path}");
                return null;
            }

            return ImageTexture.CreateFromImage(image);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RegentWithASteelPipe] Failed to load texture '{path}': {ex.Message}");
            return null;
        }
    }
}
