namespace Trellis;

public struct Viewport
{
    public int Width { get; set; }

    public int Height { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public readonly int HorizontalExtent => X + Width;

    public readonly int VerticalExtent => Y + Height;

    public readonly int Area => Width * Height;
}
