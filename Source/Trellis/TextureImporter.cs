using FlaxEngine;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Trellis;

public sealed class TextureImporter(Color32[] pixels, TextureInfo info)
{
    public ImmutableArray<Color32> Pixels { get; init; } = ImmutableArray.Create(pixels);

    public TextureInfo Info { get; init; } = info;

    public TextureChannel ExtractChannel(ChannelMask channelMask)
    {
        int channelIndex = (int)channelMask;

        byte[] pixels = GC.AllocateUninitializedArray<byte>(Info.PixelCount);
        var enumerator = Pixels.GetEnumerator();
        for (int i = 0; enumerator.MoveNext(); i++)
        {
            pixels[i] = enumerator.Current[channelIndex];
        }

        return new(pixels, Info, channelMask);
    }

    public static async Task<TextureImporter> ImportAsync(Texture texture, int mipIndex = 0, int arrayIndex = 0)
    {
        Color32[] pixels = await Task.Run(() => GetPixels(texture, mipIndex, arrayIndex));
        TextureInfo info = new(texture);
        return new TextureImporter(pixels, info);
    }
    
    public static TextureImporter Import(Texture texture, int mipIndex = 0, int arrayIndex = 0)
    {       
        Color32[] pixels = GetPixels(texture, mipIndex, arrayIndex);
        TextureInfo info = new(texture);
        return new TextureImporter(pixels, info);
    }

    private static Color32[] GetPixels(Texture texture, int mipIndex = 0, int arrayIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(texture, nameof(texture));
        if (texture.GetPixels(out Color32[] pixels, mipIndex, arrayIndex))
        {
            throw new InvalidOperationException($"Unable to read pixels from {texture}.");
        }

        return pixels;
    }
}
