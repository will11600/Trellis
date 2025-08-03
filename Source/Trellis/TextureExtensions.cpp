#include "TextureExtensions.h"
#include "Engine/Graphics/PixelFormatExtensions.h"
#include "Engine/Graphics/PixelFormatSampler.h"
#include "Engine/Core/Math/Color.h"
#include "Engine/Core/Math/Math.h"
#include "Engine/Graphics/Textures/TextureData.h"
#include "Engine/Core/Math/Vector2.h"

Color TextureExtensions::SampleAt(API_PARAM(this) TextureSampler& sampler, const Float2& uv)
{
	return sampler.sampler->SampleLinear(sampler.mipData->Data.begin(), uv, sampler.size, sampler.mipData->RowPitch);
}

TextureSampler TextureExtensions::CreateSampler(API_PARAM(this)Texture& texture, int32 mipIndex, int32 arrayIndex)
{
	TextureSampler reader;

	TextureMipData mipData;
	if (texture.GetTextureMipData(mipData, mipIndex, arrayIndex, false))
	{
		return reader;
	}

	reader.lock = &texture.LockData();
	reader.sampler = PixelFormatSampler::Get(texture.Format());
	reader.mipData = &mipData;
	reader.size = texture.Size();

	return reader;
}

API_FUNCTION() void TextureExtensions::DisposeSampler(API_PARAM(this) TextureSampler& sampler)
{
	sampler.lock->Release();
}

