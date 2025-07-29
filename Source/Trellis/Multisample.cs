using FlaxEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Trellis;

internal unsafe struct Multisample : IEnumerable<byte>
{
    public const int Length = 4;
    public const int Count = Length * Length;

    private fixed byte _samples[Count];

    public byte this[int index]
    {
        readonly get
        {
            CheckBounds(in index);
            return _samples[index];
        }
        set
        {
            CheckBounds(in index);
            _samples[index] = value;
        }
    }

    public unsafe byte this[int x, int y]
    {
        readonly get
        {
            CheckBounds(in x, in y);
            return _samples[y * Length + x];
        }
        set
        {
            CheckBounds(in x, in y);
            _samples[y * Length + x] = value;
        }
    }

    public readonly byte SampleBicubic(float u, float v)
    {
        float x = Mathf.CubicInterp(this[0, 0], this[1, 0], this[2, 0], this[3, 0], u);
        float y = Mathf.CubicInterp(this[0, 1], this[1, 1], this[2, 1], this[3, 1], u);
        float z = Mathf.CubicInterp(this[0, 2], this[1, 2], this[2, 2], this[3, 2], u);
        float w = Mathf.CubicInterp(this[0, 3], this[1, 3], this[2, 3], this[3, 3], u);
        return (byte)Mathf.CubicInterp(x, y, z, w, v);
    }

    public readonly IEnumerator<byte> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    readonly IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static void CheckBounds(scoped ref readonly int index, int count = Count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, count, nameof(index));
        ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
    }

    private static void CheckBounds(scoped ref readonly int x, scoped ref readonly int y)
    {
        CheckBounds(in x, Length);
        CheckBounds(in y, Length);
    }
}
