using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trellis.PixelMapping;

namespace Trellis;

public struct LayerChannel : IReadOnlyCollection<byte>
{
    internal readonly Multisample[,] pixels;
    private readonly TextureInfo _info;

    public readonly byte this[float u, float v]
    {
        get
        {
            int x = PixelMapper.UToX(u, out float xRemainder);
            int y = PixelMapper.VToY(v, out float yRemainder);
            return pixels[x, y].SampleBicubic(xRemainder, yRemainder);
        }
    }

    public readonly byte this[int x, int y]
    {
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
            ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, _info.PixelWidth, nameof(x));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, _info.PixelHeight, nameof(y));

            int length = Multisample.Length;
            int xIndex = x / length;
            int yIndex = y / length;

            Multisample multisample = pixels[xIndex, yIndex];
            multisample[x % length, y % length] = value;
            pixels[xIndex, yIndex] = multisample;
        }
    }

    public Layer Layer { get; init; }

    public readonly int Count => _info.SampleCount;

    public IPixelMapper PixelMapper { get; set; }

    public LayerChannel(Layer layer, IPixelMapper pixelMapper, IReadOnlyList<byte> pixelData)
    {
        _info = pixelMapper.Info;

        Layer = layer;
        PixelMapper = pixelMapper;

        Multisample[,] channelPixels = new Multisample[pixelMapper.Info.ResampledWidth, pixelMapper.Info.ResampledHeight];
        Parallel.For(0, pixelMapper.Info.SampleCount, i => AddSample(channelPixels, pixelData, i));
        pixels = channelPixels;
    }

    private static void AddSample(Multisample[,] channelPixels, IReadOnlyList<byte> texturePixels, int index)
    {
        Multisample sample = new();

        int width = channelPixels.GetLength(0);

        int x = index % width;
        int y = index / width;

        int textureWidth = width * Multisample.Length;

        int textureX = x * Multisample.Length;
        int textureY = y * Multisample.Length;

        for (int sx = 0; sx < Multisample.Length; sx++)
        {
            for (int sy = 0; sy < Multisample.Length; sy++)
            {
                int px = sx + textureX;
                int py = sy + textureY;

                sample[sx, sy] = texturePixels[py * textureWidth + px];
            }
        }

        channelPixels[x, y] = sample;
    }

    public readonly IEnumerator<byte> GetEnumerator()
    {
        foreach (var multisample in pixels)
        {
            yield return multisample.SampleBicubic(0.5f, 0.5f);
        }
    }

    readonly IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
