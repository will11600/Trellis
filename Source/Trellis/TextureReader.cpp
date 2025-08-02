#include "TextureReader.h"
#include <Engine/Core/Math/Math.h>
#include <Engine/Graphics/Textures/TextureData.h>
#include <Engine/Core/Log.h>
#include <Engine/Graphics/PixelFormatExtensions.h>
#include <stdexcept>

int32 TextureReader::BytesPerPixel() const
{
    return _bytesPerPixel;
}

int32 TextureReader::Width() const
{
    return _width;
}

int32 TextureReader::Height() const
{
    return _height;
}

TextureReader::TextureReader(Texture* texture, int32 mipIndex, int32 arrayIndex) : _dataLock(texture->LockData())
{
    _texture = texture;

    _mipIndex = mipIndex;
    _arrayIndex = arrayIndex;

    _width = Math::Max(1, texture->Width() >> mipIndex);
    _height = Math::Max(1, texture->Height() >> mipIndex);

    _bytesPerPixel = PixelFormatExtensions::SizeInBytes(texture->Format());

    if (texture->GetTextureMipData(_mipData, _mipIndex, _arrayIndex, false))
    {
        throw std::runtime_error("Failed to open texture data for reading.");
    }
}

TextureReader::~TextureReader()
{
    _dataLock.Release();
}

int32 TextureReader::Read(int32 x, int32 y, Span<byte> buffer)
{
    if (x >= _width || x < 0)
    {
        LOG(Error, "The index of 'x' is out of range. Expected: 0-{0} Received: {1}", _width, x);
        return -1;
    }

    if (y >= _height || y < 0)
    {
        LOG(Error, "The index of 'y' is out of range. Expected: 0-{0} Received: {1}", _height, y);
        return -1;
    }
    
    int32 rowStride = _width * _bytesPerPixel;
    byte* rowStart = _mipData.Data.begin() + (y * rowStride);
    byte* pixelStart = rowStart + (x * _bytesPerPixel);

    int32 concatenatedLength = Math::Min(_bytesPerPixel, buffer.Length());

    memcpy(buffer.begin(), pixelStart, concatenatedLength);
    
    return concatenatedLength;
}