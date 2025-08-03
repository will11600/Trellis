using FlaxEngine;
using System;
using System.Collections.Generic;

namespace Trellis;

public sealed class Canvas(Color32 backgroundColor) : List<Layer>()
{
    public Color Background { get; set; } = backgroundColor;

    public Color[] Render(Viewport viewport)
    {
        Color[] pixels = new Color[viewport.Width * viewport.Height];
        float backgroundAlpha = Background.A / byte.MaxValue;

        if (Count == 0)
        {
            return pixels;
        }

        int horizontalExtent = viewport.HorizontalExtent;
        int verticalExtent = viewport.VerticalExtent;

        Span<TextureSampler> textureSamplers = stackalloc TextureSampler[Count];
        for (int i = 0; i < Count; i++)
        {
            textureSamplers[i] = this[i].Texture.CreateSampler();
        }

        try
        {
            int pixelIndex = 0;
            for (int x = viewport.X; x < horizontalExtent; x++)
            {
                for (int y = viewport.Y; y < verticalExtent; y++)
                {
                    Color pixel = Background;
                    Float2 location = new(x, y);

                    for (int i = 0; i < Count; i++)
                    {
                        this[i].Render(in textureSamplers[i], in location, ref pixel, backgroundAlpha);
                    }

                    pixels[pixelIndex++] = pixel;
                }
            }
        }
        finally
        {
            for (int i = 0; i < Count; i++)
            {
                textureSamplers[i].Dispose();
            }
        }

        return pixels;
    }
}
