namespace Trellis.PixelMapping;

public interface IPixelMapper
{
    public TextureInfo Info { get; init; }

    public int UToX(float u, out float remainder);

    public int VToY(float v, out float remainder);
}