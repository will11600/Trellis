using System;
using System.Runtime.InteropServices;
using FlaxEngine;

namespace Trellis;

/// <summary>
/// Provides methods for reading pixel data from a <see cref="Texture"/>.
/// </summary>
public static partial class TextureReader
{
    /// <summary>
    /// Reads a single pixel of a specified unmanaged type <typeparamref name="T"/> from the texture at the given coordinates.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of the pixel data to read. The size of this type must match the size of the texture's pixel format.</typeparam>
    /// <param name="texture">The texture to read from. Cannot be null.</param>
    /// <param name="x">The x-coordinate (horizontal) of the pixel to read.</param>
    /// <param name="y">The y-coordinate (vertical) of the pixel to read.</param>
    /// <param name="mipIndex">The mipmap level to read from. Defaults to 0.</param>
    /// <param name="arrayIndex">The array slice to read from (for texture arrays). Defaults to 0.</param>
    /// <returns>The pixel data as a value of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="texture"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="x"/> or <paramref name="y"/> are outside the dimensions of the specified <paramref name="mipIndex"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the texture data cannot be read, which may indicate that the texture has not been loaded successfully or is in an invalid state.</exception>
    public static unsafe T ReadAs<T>(Texture texture, int x, int y, int mipIndex = 0, int arrayIndex = 0) where T : unmanaged
    {
        var (typeSize, bytesPerPixel) = GetSize<T>(texture);
        Span<byte> buffer = stackalloc byte[Math.Max(typeSize, bytesPerPixel)];
        Read(texture, buffer[..bytesPerPixel], x, y, mipIndex, arrayIndex);
        return MemoryMarshal.Read<T>(buffer[..typeSize]);
    }

    /// <summary>
    /// Reads a single pixel of a specified unmanaged type <typeparamref name="T"/> from the texture at the given coordinates, ensuring the size of <typeparamref name="T"/> exactly matches the texture's pixel format size.
    /// </summary>
    /// <typeparam name="T">The unmanaged type of the pixel data to read. The size of this type must exactly match the size of the texture's pixel format.</typeparam>
    /// <param name="texture">The texture to read from. Cannot be null.</param>
    /// <param name="x">The x-coordinate (horizontal) of the pixel to read.</param>
    /// <param name="y">The y-coordinate (vertical) of the pixel to read.</param>
    /// <param name="mipIndex">The mipmap level to read from. Defaults to 0.</param>
    /// <param name="arrayIndex">The array slice to read from (for texture arrays). Defaults to 0.</param>
    /// <returns>The pixel data as a value of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="texture"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="x"/> or <paramref name="y"/> are outside the dimensions of the specified <paramref name="mipIndex"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if the size of <typeparamref name="T"/> does not exactly match the texture's pixel format size.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the texture data cannot be read, which may indicate that the texture has not been loaded successfully or is in an invalid state.</exception>
    public static unsafe T ReadExactlyAs<T>(Texture texture, int x, int y, int mipIndex = 0, int arrayIndex = 0) where T : unmanaged
    {
        var (typeSize, bytesPerPixel) = GetSize<T>(texture);

        if (bytesPerPixel != typeSize)
        {
            throw new ArgumentException($"The size of the specified type '{typeof(T).Name}' ({typeSize} bytes) does not match the texture's pixel format size ({bytesPerPixel} bytes).", nameof(T));
        }

        Span<byte> buffer = stackalloc byte[bytesPerPixel];
        Read(texture, buffer, x, y, mipIndex, arrayIndex);
        return MemoryMarshal.Read<T>(buffer);
    }

    /// <summary>
    /// Reads a single pixel of from the texture at the given coordinates into the provided <paramref name="buffer"/>.
    /// </summary>
    /// <param name="texture">The texture to read from. Cannot be null.</param>
    /// <param name="buffer">The buffer to read pixel data into.</param>
    /// <param name="x">The x-coordinate (horizontal) of the pixel to read.</param>
    /// <param name="y">The y-coordinate (vertical) of the pixel to read.</param>
    /// <param name="mipIndex">The mipmap level to read from. Defaults to 0.</param>
    /// <param name="arrayIndex">The array slice to read from (for texture arrays). Defaults to 0.</param>
    /// <returns>The pixel data as a value of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="texture"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="x"/> or <paramref name="y"/> are outside the dimensions of the specified <paramref name="mipIndex"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the texture data cannot be read, which may indicate that the texture has not been loaded successfully or is in an invalid state.</exception>
    public static unsafe void Read(Texture texture, Span<byte> buffer, int x, int y, int mipIndex = 0, int arrayIndex = 0)
    {
        fixed (byte* ptr = buffer)
        {
            switch (Read(texture, mipIndex, arrayIndex, ptr, buffer.Length, x, y))
            {
                case 0:
                    return;
                case 1:
                    throw new ArgumentOutOfRangeException(nameof(x), x, $"The x-coordinate ({x}) is outside the valid range of the texture's width for mip level {mipIndex}.");
                case 2:
                    throw new ArgumentOutOfRangeException(nameof(y), y, $"The y-coordinate ({y}) is outside the valid range of the texture's height for mip level {mipIndex}.");
                case 3:
                    throw new ArgumentNullException(nameof(texture));
                default:
                    throw new InvalidOperationException("Failed to read from the texture. The texture may not be loaded or may be in an invalid state.");
            };
        }
    }

    private static unsafe (int typeSize, int bytesPerPixel) GetSize<T>(Texture texture) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(texture, nameof(texture));
        return (sizeof(T), PixelFormatExtensions.SizeInBytes(texture.Format));
    }
}