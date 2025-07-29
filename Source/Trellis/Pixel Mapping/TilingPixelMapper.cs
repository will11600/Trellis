using FlaxEngine;

namespace Trellis.PixelMapping;

/// <summary>
/// TilingPixelMapper class.
/// </summary>
public sealed class TilingPixelMapper : IPixelMapper
{
    /// <inheritdoc/>
    public TextureInfo Info { get; init; }

    /// <inheritdoc/>
    public int UToX(float u, out float remainder)
    {
        if (Info?.ResampledWidth == 0)
        {
            remainder = 0f;
            return 0;
        }

        int width = Info.ResampledWidth - 1;
        float scaledU = u * width / Multisample.Length;
        float xFloor = Mathf.Floor(scaledU);
        remainder = scaledU - xFloor;

        int x = (int)xFloor % width;
        if (x < 0)
        {
            x += width;
        }
        return x;
    }

    /// <inheritdoc/>
    public int VToY(float v, out float remainder)
    {
        if (Info?.ResampledHeight == 0)
        {
            remainder = 0f;
            return 0;
        }

        int height = Info.ResampledHeight - 1;
        float scaledV = v * height / Multisample.Length;
        float yFloor = Mathf.Floor(scaledV);
        remainder = scaledV - yFloor;

        int y = (int)yFloor % height;
        if (y < 0)
        {
            y += height;
        }
        return y;
    }
}