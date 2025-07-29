using FlaxEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trellis.Blending;
using Trellis.PixelFormats;
using Trellis.PixelMapping;

namespace Trellis;

public sealed class Layer : IReadOnlyCollection<LayerChannel>, IRenderable
{
    private const int ChannelCount = 4;
    private static readonly ColorBlendLinear DefaultBlendMode = new();
    private readonly BitArray _bitmask = new(ChannelCount);

    public Rectangle Transform { get; set; }

    public IColorBlendMode BlendMode { get; set; }

    public int Count { get; private set; }

    private LayerChannel _r;
    public LayerChannel R
    {
        get => _r;
        set
        {
            _r = value;
            _bitmask[0] = true;
            Count = CountLayers();
        }
    }

    private LayerChannel _g;
    public LayerChannel G
    {
        get => _g;
        set
        {
            _g = value;
            _bitmask[1] = true;
            Count = CountLayers();
        }
    }

    private LayerChannel _b;
    public LayerChannel B
    {
        get => _b;
        set
        {
            _b = value;
            _bitmask[2] = true;
            Count = CountLayers();
        }
    }

    private LayerChannel _a;
    public LayerChannel A
    {
        get => _a;
        set
        {
            _a = value;
            _bitmask[3] = true;
            Count = CountLayers();
        }
    }

    public LayerChannel this[ChannelMask channelMask]
    {
        get => this[(int)channelMask];
        set => this[(int)channelMask] = value;
    }

    public LayerChannel this[int index]
    {
        get => GetLayerRef(index);
        set
        {
            GetLayerRef(index) = value;
            _bitmask[index] = true;
            Count = CountLayers();
        }
    }

    private ref LayerChannel GetLayerRef(int index)
    {
        switch (index)
        {
            case 0: return ref _r;
            case 1: return ref _g;
            case 2: return ref _b;
            case 3: return ref _a;
            default: throw new IndexOutOfRangeException();
        }
    }

    public bool HasChannel(ChannelMask channelMask)
    {
        return _bitmask[(int)channelMask];
    }
    
    public bool HasChannel(int channelIndex)
    {
        return channelIndex > 0 && channelIndex < ChannelCount && _bitmask[channelIndex];
    }

    public bool TryGetChannel(int channelIndex, out LayerChannel result)
    {
        if (channelIndex < 0 || channelIndex >= ChannelCount)
        {
            result = default;
            return false;
        }

        result = GetLayerRef(channelIndex);
        return _bitmask[channelIndex];
    }

    public bool TryGetChannel(ChannelMask channelMask, out LayerChannel result)
    {
        int index = (int)channelMask;
        result = GetLayerRef(index);
        return _bitmask[index];
    }

    public Color32 SampleAt(float u, float v)
    {
        Color32 sample = new();
        Parallel.For(0, ChannelCount, i =>
        {
            if (_bitmask[i])
            {
                sample[i] = GetLayerRef(i)[u, v];
                return;
            }

            sample[i] = i == (int)ChannelMask.Alpha ? byte.MaxValue : byte.MinValue;
        });

        return sample;
    }

    public byte SampleAt(float u, float v, ChannelMask channelMask)
    {
        int index = (int)channelMask;

        if (_bitmask[index])
        {
            return GetLayerRef(index)[u, v];
        }

        return index == (int)ChannelMask.Alpha ? byte.MaxValue : byte.MinValue;
    }

    public void Render(int x, int y, ref Color32 pixel, float backgroundAlpha)
    {
        float maxValue = byte.MaxValue;

        if (!IsVisible(x, y))
        {
            return;
        }

        var (u, v) = CalculateUVCoordinates(x, y);

        Color32 sample = SampleAt(u, v);

        if (sample.A == byte.MinValue)
        {
            return;
        }

        float alpha = PorterDuffAlphaBlend(pixel.A / maxValue, sample.A / maxValue, backgroundAlpha);

        IColorBlendMode colorBlendMode = BlendMode ?? DefaultBlendMode;
        pixel = colorBlendMode.Blend((RGB32)sample, (RGB32)pixel, alpha);
        pixel.A = (byte)(alpha * maxValue);
    }

    public void Render(int x, int y, ref RA32 pixel, float backgroundAlpha, ChannelMask channelMask)
    {
        float maxValue = byte.MaxValue;

        if (!IsVisible(x, y))
        {
            return;
        }

        var (u, v) = CalculateUVCoordinates(x, y);

        byte sampleValue = SampleAt(u, v, channelMask);
        byte sampleAlpha = SampleAt(u, v, ChannelMask.Alpha);

        if (sampleAlpha == byte.MinValue)
        {
            return;
        }

        float alpha = PorterDuffAlphaBlend(pixel.a / maxValue, sampleAlpha / maxValue, backgroundAlpha);

        IColorBlendMode colorBlendMode = BlendMode ?? DefaultBlendMode;
        byte blendedValue = colorBlendMode.Blend(sampleValue, pixel.r, alpha);

        pixel = new(blendedValue, (byte)(alpha * maxValue));
    }

    private bool IsVisible(int x, int y)
    {
        Float2 location = new(x, y);
        if (Transform.Size.IsZero || !Transform.Contains(location))
        {
            return false;
        }

        return _bitmask.HasAnySet();
    }

    private (float u, float v) CalculateUVCoordinates(int x, int y)
    {
        return ((x - Transform.X) / Transform.Width, (y - Transform.Y) / Transform.Height);
    }

    private int CountLayers()
    {
        int count = 0;
        for (int i = 0; i < ChannelCount; i++)
        {
            if (!_bitmask[i])
            {
                continue;
            }

            count++;
        }
        return count;
    }

    private static float PorterDuffAlphaBlend(float bottom, float top, float backgroundAlpha)
    {
        return top * bottom + (1 - top) * backgroundAlpha;
    }

    public static Layer CreateFromTexture<T>(Texture texture) where T : class, IPixelMapper, new()
    {
        TextureImporter importer = TextureImporter.Import(texture);
        return CreateFromTexture<T>(importer, DefaultBlendMode);
    }

    public static async Task<Layer> CreateFromTextureAsync<T>(Texture texture) where T : class, IPixelMapper, new()
    {
        TextureImporter importer = await TextureImporter.ImportAsync(texture);
        return CreateFromTexture<T>(importer, DefaultBlendMode);
    }
    
    public static Layer CreateFromTexture<T>(Texture texture, IColorBlendMode colorBlendMode) where T : class, IPixelMapper, new()
    {
        TextureImporter importer = TextureImporter.Import(texture);
        return CreateFromTexture<T>(importer, colorBlendMode);
    }

    public static async Task<Layer> CreateFromTextureAsync<T>(Texture texture, IColorBlendMode colorBlendMode) where T : class, IPixelMapper, new()
    {
        TextureImporter importer = await TextureImporter.ImportAsync(texture);
        return CreateFromTexture<T>(importer, colorBlendMode);
    }

    private static Layer CreateFromTexture<T>(TextureImporter importer, IColorBlendMode colorBlendMode) where T : class, IPixelMapper, new()
    {
        IPixelMapper pixelMapper = new T
        {
            Info = importer.Info
        };

        Layer layer = new()
        {
            Transform = new Rectangle(0f, 0f, importer.Info.PixelWidth, importer.Info.PixelHeight),
            BlendMode = colorBlendMode
        };

        Parallel.For(0, ChannelCount, i =>
        {
            TextureChannel textureChannel = importer.ExtractChannel((ChannelMask)i);
            layer[i] = new(layer, pixelMapper, textureChannel.Pixels);
        });

        layer.Transform = new Rectangle(0f, 0f, importer.Info.ResampledWidth, importer.Info.ResampledHeight);

        return layer;
    }

    public IEnumerator<LayerChannel> GetEnumerator()
    {
        for (int i = 0; i < ChannelCount; i++)
        {
            if (!_bitmask[i])
            {
                continue;
            }

            yield return GetLayerRef(i);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
