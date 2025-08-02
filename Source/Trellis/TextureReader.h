#pragma once

#include "Engine/Scripting/Script.h"
#include <Engine/Content/Assets/Texture.h>
#include <Engine/Graphics/Textures/TextureData.h>

/// <summary>
/// A utility class for reading pixel data from a texture.
/// </summary>
class TextureReader
{
private:
    Texture* _texture;
    int32 _mipIndex;
    int32 _arrayIndex;
    int32 _bytesPerPixel;
    int32 _width;
    int32 _height;
    TextureMipData _mipData;
    FlaxStorage::LockData _dataLock;
public:
    /// <summary>
    /// Gets the number of bytes per pixel for the texture's format.
    /// </summary>
    /// <returns>The number of bytes per pixel.</returns>
    int32 BytesPerPixel() const;
    /// <summary>
    /// Gets the width of the texture at the current mipmap level.
    /// </summary>
    /// <returns>The width of the texture.</returns>
    int32 Width() const;
    /// <summary>
    /// Gets the height of the texture at the current mipmap level.
    /// </summary>
    /// <returns>The height of the texture.</returns>
    int32 Height() const;
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureReader"/> class.
    /// </summary>
    /// <param name="texture">The texture to read from.</param>
    /// <param name="mipIndex">The mipmap level to read.</param>
    /// <param name="arrayIndex">The array slice to read (for texture arrays).</param>
    /// <exception cref="runtime_error">Thrown if the provided <paramref name="texture"/> cannot be read.</exception>
    TextureReader(Texture* texture, int32 mipIndex, int32 arrayIndex);
    /// <summary>
    /// Reads a single pixel from the specified coordinates into a buffer.
    /// </summary>
    /// <param name="x">The x-coordinate of the pixel.</param>
    /// <param name="y">The y-coordinate of the pixel.</param>
    /// <param name="buffer">The buffer to store the pixel data. The buffer should be at least as large as BytesPerPixel().</param>
    /// <returns>The number of bytes read, or -1 if the coordinates are out of bounds.</returns>
    int32 Read(int32 x, int32 y, Span<byte> buffer);
    /// <summary>
    /// Finalizes an instance of the <see cref="TextureReader"/> class, ensuring resources are released.
    /// </summary>
    ~TextureReader();
};