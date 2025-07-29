using FlaxEngine;
using System;

namespace Trellis;

public sealed class TextureInfo : IEquatable<TextureInfo>
{
    private readonly Texture _texture;

    public int PixelWidth => _texture.Width;

    public int PixelHeight => _texture.Height;

    public int PixelCount { get; init; }

    public int ResampledWidth { get; init; }

    public int ResampledHeight { get; init; }

    public int SampleCount { get; init; }

    public TextureInfo(Texture texture)
    {
        _texture = texture;
        
        int width = _texture.Width;
        int height = _texture.Height;

        PixelCount = width * height;
        ResampledWidth = width / Multisample.Length;
        ResampledHeight = height / Multisample.Length;
        SampleCount = width * height / Multisample.Count;
    }

    public bool Equals(TextureInfo other)
    {
        return _texture.Equals(other._texture);
    }

    public override bool Equals(object obj)
    {
        return obj is TextureInfo textureData && Equals(textureData);
    }

    public override int GetHashCode()
    {
        return _texture.GetHashCode();
    }

    public override string ToString()
    {
        return _texture.ToString();
    }
}
