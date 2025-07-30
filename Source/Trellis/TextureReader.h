#pragma once

#include "Engine/Scripting/Script.h"
#include <Engine/Content/Assets/Texture.h>
#include <Engine/Graphics/Textures/TextureData.h>

/// <summary>
/// Provides methods for reading pixel data from a <see cref="Texture"/>.
/// </summary>
API_CLASS(Static) class TRELLIS_API TextureReader
{
	DECLARE_SCRIPTING_TYPE_MINIMAL(TextureReader);
	private:
        /// <summary>
        /// Reads a single pixel's data from a specified texture at a given MipMap level and array slice.
        /// </summary>
        /// <param name="texture">The texture to read from.</param>
        /// <param name="mipIndex">The MipMap level to read from (0 for the highest resolution).</param>
        /// <param name="arrayIndex">The array slice index for 3D textures or texture arrays.</param>
        /// <param name="data">A pointer to a byte array where the pixel data will be copied. This array must be large enough to hold the pixel's data (e.g., `bytesPerPixel`).</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel</param>
        /// <param name="x">The X-coordinate of the pixel to read.</param>
        /// <param name="y">The Y-coordinate of the pixel to read.</param>
        /// <returns>
        /// Returns 0 on success (EXIT_SUCCESS).
        /// Returns 1 if the X-coordinate is out of bounds (EXIT_FAILURE_X_OUT_OF_RANGE).
        /// Returns 2 if the Y-coordinate is out of bounds (EXIT_FAILURE_Y_OUT_OF_RANGE).
        /// Returns 3 if the texture pointer is null (EXIT_FAILURE_NULL_PTR).
        /// Returns 4 if the texture mip data could not be loaded (EXIT_FAILURE_LOAD).
        /// </returns>
		API_FUNCTION() static int32 Read(Texture* texture, int32 mipIndex, int32 arrayIndex, byte* data, int32 bytesPerPixel, int32 x, int32 y);
};
