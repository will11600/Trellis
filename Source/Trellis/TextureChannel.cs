using FlaxEngine;
using System.Collections.Immutable;

namespace Trellis;

public readonly struct TextureChannel(byte[] pixels, TextureInfo info, ChannelMask channelMask)
{
    public ImmutableArray<byte> Pixels { get; init; } = ImmutableArray.Create(pixels);

    public TextureInfo Info { get; init; } = info;

    public ChannelMask Channel { get; init; } = channelMask;
}
