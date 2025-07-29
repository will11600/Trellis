using FlaxEngine;
using Trellis.PixelFormats;

namespace Trellis;

public interface IRenderable
{
    public void Render(int x, int y, ref Color32 pixel, float backgroundAlpha);

    public void Render(int x, int y, ref RA32 pixel, float backgroundAlpha, ChannelMask channelMask);
}
