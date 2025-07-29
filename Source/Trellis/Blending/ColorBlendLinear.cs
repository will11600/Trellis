using FlaxEngine;
using Trellis.PixelFormats;

namespace Trellis.Blending;

public sealed class ColorBlendLinear : IColorBlendMode
{
    public RGB32 Blend(RGB32 top, RGB32 bottom, float alpha)
    {
        float r = Mathf.Lerp(bottom.r, top.r, alpha);
        float g = Mathf.Lerp(bottom.g, top.g, alpha);
        float b = Mathf.Lerp(bottom.b, top.b, alpha);
        return new RGB32((byte)r, (byte)g, (byte)b);
    }

    public byte Blend(byte top, byte bottom, float alpha)
    {
        float r = Mathf.Lerp(bottom, top, alpha);
        return (byte)r;
    }
}
