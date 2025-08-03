#include "TextureSampler.h"
#include "Engine/Core/Math/Color.h"
#include "Engine/Graphics/PixelFormatSampler.h"
#include "Engine/Graphics/Textures/TextureData.h"
#include "memory"

TextureSampler::TextureSampler() : size(Float2::Zero)
{
	lock = nullptr;
	sampler = nullptr;
	mipData = nullptr;
}
