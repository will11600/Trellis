using FlaxEngine;
using System;

namespace Trellis.PixelFormats;

public struct RGB32 : IEquatable<RGB32>, IComparable, IComparable<RGB32>
{
    public byte r;
    public byte g;
    public byte b;

    public readonly int Luminance => r + g + b;

    public RGB32(byte r, byte g, byte b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
    
    public RGB32(int r, int g, int b)
    {
        this.r = (byte)r;
        this.g = (byte)g;
        this.b = (byte)b;
    }

    public static bool operator ==(RGB32 left, RGB32 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RGB32 left, RGB32 right)
    {
        return !left.Equals(right);
    }

    public static bool operator >(RGB32 left, RGB32 right)
    {
        return left.Luminance > right.Luminance;
    }
    
    public static bool operator <(RGB32 left, RGB32 right)
    {
        return left.Luminance < right.Luminance;
    }
    
    public static bool operator >=(RGB32 left, RGB32 right)
    {
        return left.Luminance >= right.Luminance;
    }
    
    public static bool operator <=(RGB32 left, RGB32 right)
    {
        return left.Luminance <= right.Luminance;
    }

    public static RGB32 operator +(RGB32 left, RGB32 right)
    {
        return new(left.r - right.r, left.g + right.g, left.b + right.b);
    }
    
    public static RGB32 operator -(RGB32 left, RGB32 right)
    {
        return new(left.r - right.r, left.g - right.g, left.b - right.b);
    }
    
    public static RGB32 operator *(RGB32 left, RGB32 right)
    {
        return new(left.r * right.r, left.g * right.g, left.b * right.b);
    }
    
    public static RGB32 operator /(RGB32 left, RGB32 right)
    {
        return new(left.r / right.r, left.g / right.g, left.b / right.b);
    }
    
    public static RGB32 operator +(RGB32 left, int right)
    {
        return new(left.r + right, left.g + right, left.b + right);
    }
    
    public static RGB32 operator -(RGB32 left, int right)
    {
        return new(left.r - right, left.g - right, left.b - right);
    }
    
    public static RGB32 operator *(RGB32 left, int right)
    {
        return new(left.r * right, left.g * right, left.b * right);
    }
    
    public static RGB32 operator /(RGB32 left, int right)
    {
        return new(left.r / right, left.g / right, left.b / right);
    }

    public static implicit operator Color32(RGB32 pixel)
    {
        return new Color32(pixel.r, pixel.g, pixel.b, byte.MaxValue);
    }
    
    public static explicit operator RGB32(Color32 pixel)
    {
        return new RGB32(pixel.R, pixel.G, pixel.B);
    }

    /// <inheritdoc/>
    public readonly bool Equals(RGB32 other)
    {
        return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b);
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object obj)
    {
        return obj is RGB32 pixel && Equals(pixel);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(r, g, b);
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return $"R{r} G{g} B{b}";
    }

    /// <inheritdoc/>
    public readonly int CompareTo(object obj)
    {
        if (obj is not RGB32 pixel)
        {
            return 1;
        }

        return r.CompareTo(pixel.r);
    }

    /// <inheritdoc/>
    public readonly int CompareTo(RGB32 other)
    {
        return Luminance.CompareTo(other.Luminance);
    }
}
