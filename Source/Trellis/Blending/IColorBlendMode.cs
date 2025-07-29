using Trellis.PixelFormats;

namespace Trellis.Blending;

public interface IColorBlendMode
{
    public RGB32 Blend(RGB32 top, RGB32 bottom, float alpha);

    public byte Blend(byte top, byte bottom, float alpha);
}
