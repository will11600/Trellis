using FlaxEngine;
using System;
using System.Runtime.CompilerServices;

namespace Trellis;

public sealed class Layer : IRenderable
{
    public Texture Texture { get; init; }

    public int ChannelCount { get; init; }

    public Rectangle Transform { get; set; }

    public Layer(Texture texture)
    {
        ArgumentNullException.ThrowIfNull(texture, nameof(texture));
        Texture = texture;

        ChannelCount = PixelFormatExtensions.ComputeComponentsCount(texture.Format);

        Transform = Rectangle.Empty;
    }

    public void Render(scoped ref readonly TextureSampler textureSampler, scoped ref readonly Float2 location, ref Color pixel, float backgroundAlpha)
    {
        if (ChannelCount < 1 || Transform.Size.IsZero || !Transform.Contains(location))
        {
            return;
        }

        Float2 uv = (location - Transform.Location) / Transform.Size;
        Color sample = textureSampler.SampleAt(uv);

        if (sample.A == 0f)
        {
            return;
        }

        pixel.A = PorterDuffAlphaBlend(pixel.A, sample.A, backgroundAlpha);        
        for (int i = 0; i < ChannelCount; i++)
        {
            pixel[i] = Mathf.Lerp(pixel[i], sample[i], pixel.A);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float PorterDuffAlphaBlend(float bottom, float top, float backgroundAlpha)
    {
        return top * bottom + (1 - top) * backgroundAlpha;
    }
}
