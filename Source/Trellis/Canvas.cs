using FlaxEngine;
using System.Collections.Generic;

namespace Trellis;

public sealed class Canvas(Color32 backgroundColor) : List<IRenderable>()
{
    public Color32 Background { get; set; } = backgroundColor;

    public Color32[] Render(Viewport viewport)
    {
        Color32[] pixels = new Color32[viewport.Width * viewport.Height];
        float backgroundAlpha = Background.A / (float)byte.MaxValue;

        if (Count == 0)
        {
            return pixels;
        }

        int horizontalExtent = viewport.HorizontalExtent;
        int verticalExtent = viewport.VerticalExtent;

        int i = 0;
        for (int x = viewport.X; x < horizontalExtent; x++)
        {
            for (int y = viewport.Y; y < verticalExtent; y++)
            {
                Color32 pixel = Background;

                foreach (IRenderable renderable in this)
                {
                    renderable.Render(x, y, ref pixel, backgroundAlpha);
                }

                pixels[i++] = pixel;
            }
        }

        return pixels;
    }

    //public byte[] Render(Viewport viewport, ChannelMask channelMask)
    //{
    //    byte[] pixels = new byte[viewport.Width * viewport.Height];
    //    float backgroundAlpha = Background.A / (float)byte.MaxValue;

    //    int channelIndex = TextureImporter.channels.IndexOf(channelMask);

    //    if (Count == 0 || channelIndex == -1)
    //    {
    //        return pixels;
    //    }

    //    int horizontalExtent = viewport.HorizontalExtent;
    //    int verticalExtent = viewport.VerticalExtent;

    //    int i = 0;
    //    for (int x = viewport.X; x < horizontalExtent; x++)
    //    {
    //        for (int y = viewport.Y; y < verticalExtent; y++)
    //        {
    //            byte pixel = Background[channelIndex];

    //            foreach (IRenderable renderable in this)
    //            {
    //                renderable.Render(x, y, ref pixel, backgroundAlpha, channelMask);
    //            }

    //            pixels[i++] = pixel;
    //        }
    //    }

    //    return pixels;
    //}
}
