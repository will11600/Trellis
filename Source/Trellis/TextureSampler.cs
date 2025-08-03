using System;

namespace Trellis;

public partial struct TextureSampler : IDisposable
{
    public readonly void Dispose()
    {
        TextureExtensions.DisposeSampler(this);
    }
}
