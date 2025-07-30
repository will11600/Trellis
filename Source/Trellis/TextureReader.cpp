#include "TextureReader.h"
#include <Engine/Core/Math/Math.h>
#include <Engine/Graphics/Textures/TextureData.h>

constexpr int32 EXIT_FAILURE_X_OUT_OF_RANGE = 1;
constexpr int32 EXIT_FAILURE_Y_OUT_OF_RANGE = 2;
constexpr int32 EXIT_FAILURE_NULL_PTR = 3;
constexpr int32 EXIT_FAILURE_LOAD = 4;

int32 TextureReader::Read(Texture* texture, int32 mipIndex, int32 arrayIndex, byte* data, int32 bytesPerPixel, int32 x, int32 y)
{
    if (!texture)
    {
        return EXIT_FAILURE_NULL_PTR;
    }

    auto dataLock = texture->LockData();

    int32 width = Math::Max(1, texture->Width() >> mipIndex);
    int32 height = Math::Max(1, texture->Height() >> mipIndex);

    TextureMipData mipData;
    if (texture->GetTextureMipData(mipData, mipIndex, arrayIndex, false))
    {
        return EXIT_FAILURE_LOAD;
    }

    if (x >= width || x < 0)
    {
        return EXIT_FAILURE_X_OUT_OF_RANGE;
    }

    if (y >= height || y < 0)
    {
        return EXIT_FAILURE_Y_OUT_OF_RANGE;
    }

    int32 rowStride = width * bytesPerPixel;
    byte* rowStart = mipData.Data.begin() + (y * rowStride);
    byte* pixelStart = rowStart + (x * bytesPerPixel);
    memcpy(data, pixelStart, bytesPerPixel);

    return EXIT_SUCCESS;
}