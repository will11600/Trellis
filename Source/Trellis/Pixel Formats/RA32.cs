using FlaxEngine;
using System;

namespace Trellis.PixelFormats;

public struct RA32 : IEquatable<RA32>
{
    public byte r;
    public byte a;

    public RA32(byte r, byte a)
    {
        this.r = r;
        this.a = a;
    }

    public RA32(int r, int a)
    {
        this.r = (byte)r;
        this.a = (byte)a;
    }

    public static bool operator ==(RA32 left, RA32 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RA32 left, RA32 right)
    {
        return !left.Equals(right);
    }

    public static RA32 operator +(RA32 left, RA32 right)
    {
        return new(left.r - right.r, left.a + right.a);
    }

    public static RA32 operator -(RA32 left, RA32 right)
    {
        return new(left.r - right.r, left.a - right.a);
    }

    public static RA32 operator *(RA32 left, RA32 right)
    {
        return new(left.r * right.r, left.a * right.a);
    }

    public static RA32 operator /(RA32 left, RA32 right)
    {
        return new(left.r / right.r, left.a / right.a);
    }

    public static RA32 operator +(RA32 left, int right)
    {
        return new(left.r + right, left.a + right);
    }

    public static RA32 operator -(RA32 left, int right)
    {
        return new(left.r - right, left.a - right);
    }

    public static RA32 operator *(RA32 left, int right)
    {
        return new(left.r * right, left.a * right);
    }

    public static RA32 operator /(RA32 left, int right)
    {
        return new(left.r / right, left.a / right);
    }

    public static implicit operator Color32(RA32 pixel)
    {
        return new Color32(pixel.r, byte.MinValue, byte.MinValue, pixel.a);
    }

    public static explicit operator RA32(Color32 pixel)
    {
        return new RA32(pixel.R, pixel.A);
    }

    public readonly bool Equals(RA32 other)
    {
        return r.Equals(other.r) && a.Equals(other.a);
    }

    /// <inheritdoc/>
    public override readonly bool Equals(object obj)
    {
        return obj is RA32 pixel && Equals(pixel);
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode()
    {
        return HashCode.Combine(r, a);
    }

    /// <inheritdoc/>
    public override readonly string ToString()
    {
        return $"R{r} A{a}";
    }
}
