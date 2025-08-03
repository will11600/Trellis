using FlaxEngine;

namespace Trellis;

public interface IRenderable
{
    public void Render(scoped ref readonly TextureSampler textureSampler, scoped ref readonly Float2 location, ref Color pixel, float backgroundAlpha);
}
