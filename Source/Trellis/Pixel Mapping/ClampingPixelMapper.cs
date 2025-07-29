using FlaxEngine;

namespace Trellis.PixelMapping;

public sealed class ClampingPixelMapper : IPixelMapper
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
        float scaledVal = Mathf.Saturate(u) * width;
        int xIntermediate = Mathf.FloorToInt(scaledVal);
        remainder = scaledVal - xIntermediate;

        return Mathf.Min(xIntermediate, width);
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
        float scaledVal = Mathf.Saturate(v) * height;
        int yIntermediate = Mathf.FloorToInt(scaledVal);
        remainder = scaledVal - yIntermediate;

        return Mathf.Min(yIntermediate, height);
    }
}